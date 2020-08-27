// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.ConnectionStringsManageabilityProvider
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
  public class ConnectionStringsManageabilityProvider : ConfigurationSectionManageabilityProviderBase<ConnectionStringsSection>
  {
    public const string ConnectionStringPropertyName = "connectionString";
    public const string ProviderNamePropertyName = "providerName";

    public ConnectionStringsManageabilityProvider(
      IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
      : base(subProviders)
    {
    }

    protected override void AddAdministrativeTemplateDirectives(
      AdmContentBuilder contentBuilder,
      ConnectionStringsSection configurationSection,
      IConfigurationSource configurationSource,
      string sectionKey)
    {
      contentBuilder.StartCategory(Resources.ConnectionStringsCategoryName);
      foreach (ConnectionStringSettings connectionString in (ConfigurationElementCollection) configurationSection.ConnectionStrings)
      {
        contentBuilder.StartPolicy(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ConnectionStringPolicyNameTemplate, (object) connectionString.Name), sectionKey + "\\" + connectionString.Name);
        contentBuilder.AddEditTextPart(Resources.ConnectionStringConnectionStringPartName, "connectionString", connectionString.ConnectionString, 500, true);
        contentBuilder.AddComboBoxPart(Resources.ConnectionStringProviderNamePartName, "providerName", connectionString.ProviderName, (int) byte.MaxValue, true, "System.Data.SqlClient", "System.Data.OracleClient");
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
        return "connectionStrings";
      }
    }

    protected override void OverrideWithGroupPoliciesForConfigurationSection(
      ConnectionStringsSection configurationSection,
      IRegistryKey policyKey)
    {
    }

    protected override void OverrideWithGroupPoliciesForConfigurationElements(
      ConnectionStringsSection configurationSection,
      bool readGroupPolicies,
      IRegistryKey machineKey,
      IRegistryKey userKey)
    {
      List<ConnectionStringSettings> connectionStringSettingsList = new List<ConnectionStringSettings>();
      foreach (ConnectionStringSettings connectionString in (ConfigurationElementCollection) configurationSection.ConnectionStrings)
      {
        IRegistryKey machineSubKey = (IRegistryKey) null;
        IRegistryKey userSubKey = (IRegistryKey) null;
        try
        {
          ConfigurationSectionManageabilityProvider.LoadRegistrySubKeys(connectionString.Name, machineKey, userKey, out machineSubKey, out userSubKey);
          if (!this.OverrideWithGroupPoliciesForConnectionString(connectionString, readGroupPolicies, machineSubKey, userSubKey))
            connectionStringSettingsList.Add(connectionString);
        }
        finally
        {
          ConfigurationSectionManageabilityProvider.ReleaseRegistryKeys(machineSubKey, userSubKey);
        }
      }
      foreach (ConnectionStringSettings settings in connectionStringSettingsList)
        configurationSection.ConnectionStrings.Remove(settings);
    }

    private bool OverrideWithGroupPoliciesForConnectionString(
      ConnectionStringSettings connectionString,
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
            string stringValue1 = registryKey.GetStringValue(nameof (connectionString));
            string stringValue2 = registryKey.GetStringValue("providerName");
            connectionString.ConnectionString = stringValue1;
            connectionString.ProviderName = stringValue2;
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
