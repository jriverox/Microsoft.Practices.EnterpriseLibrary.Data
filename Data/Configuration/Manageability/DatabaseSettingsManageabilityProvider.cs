// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.DatabaseSettingsManageabilityProvider
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
  public class DatabaseSettingsManageabilityProvider : ConfigurationSectionManageabilityProviderBase<DatabaseSettings>
  {
    private static readonly string[] DatabaseTypeNames = new string[3]
    {
      typeof (SqlDatabase).AssemblyQualifiedName,
      typeof (OracleDatabase).AssemblyQualifiedName,
      typeof (GenericDatabase).AssemblyQualifiedName
    };
    public const string DefaultDatabasePropertyName = "defaultDatabase";
    public const string ProviderMappingsKeyName = "providerMappings";
    public const string DatabaseTypePropertyName = "databaseType";

    public DatabaseSettingsManageabilityProvider(
      IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
      : base(subProviders)
    {
    }

    protected override void AddAdministrativeTemplateDirectives(
      AdmContentBuilder contentBuilder,
      DatabaseSettings configurationSection,
      IConfigurationSource configurationSource,
      string sectionKey)
    {
      contentBuilder.StartPolicy(Resources.DatabaseSettingsPolicyName, sectionKey);
      List<AdmDropDownListItem> dropDownListItemList = new List<AdmDropDownListItem>();
      ConnectionStringsSection section = (ConnectionStringsSection) configurationSource.GetSection("connectionStrings");
      if (section != null)
      {
        foreach (ConnectionStringSettings connectionString in (ConfigurationElementCollection) section.ConnectionStrings)
          dropDownListItemList.Add(new AdmDropDownListItem(connectionString.Name, connectionString.Name));
      }
      contentBuilder.AddDropDownListPart(Resources.DatabaseSettingsDefaultDatabasePartName, "defaultDatabase", (IEnumerable<AdmDropDownListItem>) dropDownListItemList, configurationSection.DefaultDatabase);
      contentBuilder.EndPolicy();
      if (configurationSection.ProviderMappings.Count <= 0)
        return;
      contentBuilder.StartCategory(Resources.ProviderMappingsCategoryName);
      foreach (DbProviderMapping providerMapping in configurationSection.ProviderMappings)
      {
        contentBuilder.StartPolicy(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ProviderMappingPolicyNameTemplate, (object) providerMapping.Name), sectionKey + "\\providerMappings\\" + providerMapping.Name);
        contentBuilder.AddComboBoxPart(Resources.ProviderMappingDatabaseTypePartName, "databaseType", providerMapping.DatabaseType.AssemblyQualifiedName, (int) byte.MaxValue, false, DatabaseSettingsManageabilityProvider.DatabaseTypeNames);
        contentBuilder.EndPolicy();
      }
      contentBuilder.EndCategory();
    }

    protected override string SectionCategoryName
    {
      get
      {
        return Resources.DatabaseCategoryName;
      }
    }

    protected override string SectionName
    {
      get
      {
        return "dataConfiguration";
      }
    }

    protected override void OverrideWithGroupPoliciesForConfigurationSection(
      DatabaseSettings configurationSection,
      IRegistryKey policyKey)
    {
      string stringValue = policyKey.GetStringValue("defaultDatabase");
      configurationSection.DefaultDatabase = stringValue;
    }

    protected override void OverrideWithGroupPoliciesForConfigurationElements(
      DatabaseSettings configurationSection,
      bool readGroupPolicies,
      IRegistryKey machineKey,
      IRegistryKey userKey)
    {
      List<DbProviderMapping> dbProviderMappingList = new List<DbProviderMapping>();
      IRegistryKey machineSubKey1 = (IRegistryKey) null;
      IRegistryKey userSubKey1 = (IRegistryKey) null;
      try
      {
        ConfigurationSectionManageabilityProvider.LoadRegistrySubKeys("providerMappings", machineKey, userKey, out machineSubKey1, out userSubKey1);
        foreach (DbProviderMapping providerMapping in configurationSection.ProviderMappings)
        {
          IRegistryKey machineSubKey2 = (IRegistryKey) null;
          IRegistryKey userSubKey2 = (IRegistryKey) null;
          try
          {
            ConfigurationSectionManageabilityProvider.LoadRegistrySubKeys(providerMapping.Name, machineSubKey1, userSubKey1, out machineSubKey2, out userSubKey2);
            if (!this.OverrideWithGroupPoliciesForDbProviderMapping(providerMapping, readGroupPolicies, machineSubKey2, userSubKey2))
              dbProviderMappingList.Add(providerMapping);
          }
          finally
          {
            ConfigurationSectionManageabilityProvider.ReleaseRegistryKeys(machineSubKey2, userSubKey2);
          }
        }
      }
      finally
      {
        ConfigurationSectionManageabilityProvider.ReleaseRegistryKeys(machineSubKey1, userSubKey1);
      }
      foreach (DbProviderMapping dbProviderMapping in dbProviderMappingList)
        configurationSection.ProviderMappings.Remove(dbProviderMapping.Name);
    }

    private bool OverrideWithGroupPoliciesForDbProviderMapping(
      DbProviderMapping providerMapping,
      bool readGroupPolicies,
      IRegistryKey machineKey,
      IRegistryKey userKey)
    {
      if (readGroupPolicies)
      {
        IRegistryKey registryKey = machineKey ?? userKey;
        if (registryKey != null)
        {
          if (registryKey.IsPolicyKey)
          {
            if (!registryKey.GetBoolValue("Available").Value)
              return false;
          }
          try
          {
            Type typeValue = registryKey.GetTypeValue("databaseType");
            providerMapping.DatabaseType = typeValue;
          }
          catch (RegistryAccessException ex)
          {
            this.LogExceptionWhileOverriding((Exception) ex);
          }
        }
      }
      return true;
    }
  }
}
