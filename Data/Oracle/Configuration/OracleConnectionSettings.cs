// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OracleConnectionSettings
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using System;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration
{
  [ResourceDisplayName(typeof (DesignResources), "OracleConnectionSettingsDisplayName")]
  [ResourceDescription(typeof (DesignResources), "OracleConnectionSettingsDescription")]
  public class OracleConnectionSettings : SerializableConfigurationSection
  {
    private const string oracleConnectionDataCollectionProperty = "";
    public const string SectionName = "oracleConnectionSettings";

    public static OracleConnectionSettings GetSettings(
      IConfigurationSource configurationSource)
    {
      if (configurationSource == null)
        throw new ArgumentNullException(nameof (configurationSource));
      return configurationSource.GetSection("oracleConnectionSettings") as OracleConnectionSettings;
    }

    [ConfigurationProperty("", IsDefaultCollection = true, IsRequired = false)]
    [ResourceDisplayName(typeof (DesignResources), "OracleConnectionSettingsOracleConnectionsDataDisplayName")]
    [ConfigurationCollection(typeof (OracleConnectionData))]
    [ResourceDescription(typeof (DesignResources), "OracleConnectionSettingsOracleConnectionsDataDescription")]
    public NamedElementCollection<OracleConnectionData> OracleConnectionsData
    {
      get
      {
        return (NamedElementCollection<OracleConnectionData>) this[""];
      }
    }
  }
}
