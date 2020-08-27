// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseSettings
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using System;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration
{
  [ResourceDescription(typeof (DesignResources), "DatabaseSettingsDescription")]
  [ResourceDisplayName(typeof (DesignResources), "DatabaseSettingsDisplayName")]
  public class DatabaseSettings : SerializableConfigurationSection
  {
    private const string defaultDatabaseProperty = "defaultDatabase";
    private const string dbProviderMappingsProperty = "providerMappings";
    public const string SectionName = "dataConfiguration";

    public static DatabaseSettings GetDatabaseSettings(
      IConfigurationSource configurationSource)
    {
      if (configurationSource == null)
        throw new ArgumentNullException(nameof (configurationSource));
      return (DatabaseSettings) configurationSource.GetSection("dataConfiguration");
    }

    [ResourceDisplayName(typeof (DesignResources), "DatabaseSettingsDefaultDatabaseDisplayName")]
    [ResourceDescription(typeof (DesignResources), "DatabaseSettingsDefaultDatabaseDescription")]
    [ConfigurationProperty("defaultDatabase", IsRequired = false)]
    [Reference(typeof (ConnectionStringSettingsCollection), typeof (ConnectionStringSettings))]
    public string DefaultDatabase
    {
      get
      {
        return (string) this["defaultDatabase"];
      }
      set
      {
        this["defaultDatabase"] = (object) value;
      }
    }

    [ConfigurationCollection(typeof (DbProviderMapping))]
    [ResourceDescription(typeof (DesignResources), "DatabaseSettingsProviderMappingsDescription")]
    [ConfigurationProperty("providerMappings", IsRequired = false)]
    [ResourceDisplayName(typeof (DesignResources), "DatabaseSettingsProviderMappingsDisplayName")]
    public NamedElementCollection<DbProviderMapping> ProviderMappings
    {
      get
      {
        return (NamedElementCollection<DbProviderMapping>) this["providerMappings"];
      }
    }
  }
}
