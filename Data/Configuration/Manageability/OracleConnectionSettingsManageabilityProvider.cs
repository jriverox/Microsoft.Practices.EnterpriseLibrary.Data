// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.OracleConnectionSettingsManageabilityProvider
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
  public class OracleConnectionSettingsManageabilityProvider : ConfigurationSectionManageabilityProviderBase<OracleConnectionSettings>
  {
    public const string PackagesPropertyName = "packages";

    public OracleConnectionSettingsManageabilityProvider(
      IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
      : base(subProviders)
    {
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
        return "oracleConnectionSettings";
      }
    }

    protected override void AddAdministrativeTemplateDirectives(
      AdmContentBuilder contentBuilder,
      OracleConnectionSettings configurationSection,
      IConfigurationSource configurationSource,
      string sectionKey)
    {
      contentBuilder.StartCategory(Resources.OracleConnectionsCategoryName);
      foreach (OracleConnectionData oracleConnectionData in configurationSection.OracleConnectionsData)
      {
        string policyKey = sectionKey + "\\" + oracleConnectionData.Name;
        contentBuilder.StartPolicy(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.OracleConnectionPolicyNameTemplate, (object) oracleConnectionData.Name), policyKey);
        contentBuilder.AddEditTextPart(Resources.OracleConnectionPackagesPartName, "packages", OracleConnectionSettingsManageabilityProvider.GenerateRulesString((IEnumerable<OraclePackageData>) oracleConnectionData.Packages), 1024, true);
        contentBuilder.EndPolicy();
      }
      contentBuilder.EndCategory();
    }

    private static string GenerateRulesString(IEnumerable<OraclePackageData> packages)
    {
      KeyValuePairEncoder valuePairEncoder = new KeyValuePairEncoder();
      foreach (OraclePackageData package in packages)
        valuePairEncoder.AppendKeyValuePair(package.Name, package.Prefix);
      return valuePairEncoder.GetEncodedKeyValuePairs();
    }

    protected override void OverrideWithGroupPoliciesForConfigurationElements(
      OracleConnectionSettings configurationSection,
      bool readGroupPolicies,
      IRegistryKey machineKey,
      IRegistryKey userKey)
    {
      List<OracleConnectionData> oracleConnectionDataList = new List<OracleConnectionData>();
      foreach (OracleConnectionData connectionData in configurationSection.OracleConnectionsData)
      {
        IRegistryKey machineSubKey = (IRegistryKey) null;
        IRegistryKey userSubKey = (IRegistryKey) null;
        try
        {
          ConfigurationSectionManageabilityProvider.LoadRegistrySubKeys(connectionData.Name, machineKey, userKey, out machineSubKey, out userSubKey);
          if (!this.OverrideWithGroupPoliciesForOracleConnection(connectionData, readGroupPolicies, machineSubKey, userSubKey))
            oracleConnectionDataList.Add(connectionData);
        }
        finally
        {
          ConfigurationSectionManageabilityProvider.ReleaseRegistryKeys(machineSubKey, userSubKey);
        }
      }
      foreach (OracleConnectionData oracleConnectionData in oracleConnectionDataList)
        configurationSection.OracleConnectionsData.Remove(oracleConnectionData.Name);
    }

    private bool OverrideWithGroupPoliciesForOracleConnection(
      OracleConnectionData connectionData,
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
            string stringValue = registryKey.GetStringValue("packages");
            connectionData.Packages.Clear();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            KeyValuePairParser.ExtractKeyValueEntries(stringValue, (IDictionary<string, string>) dictionary);
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
              connectionData.Packages.Add(new OraclePackageData(keyValuePair.Key, keyValuePair.Value));
          }
          catch (RegistryAccessException ex)
          {
            this.LogExceptionWhileOverriding((Exception) ex);
          }
        }
      }
      return true;
    }

    protected override void OverrideWithGroupPoliciesForConfigurationSection(
      OracleConnectionSettings configurationSection,
      IRegistryKey policyKey)
    {
    }
  }
}
