// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent.OracleConfigurationExtension
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration;
using System;
using System.Configuration;
using System.Data.Common;
using System.Data.OracleClient;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent
{
  internal class OracleConfigurationExtension : DatabaseConfigurationExtension, IDatabaseOraclePackageConfiguration, IDatabaseOracleConfiguration, IDatabaseConfigurationProperties, IDatabaseConfigurationProviderEntry, IDatabaseConfiguration, IFluentInterface
  {
    private OracleConnectionSettings currentOracleSettings;
    private OraclePackageData currentOraclePackageData;
    private OracleConnectionData currentOracleConnectionData;

    public OracleConfigurationExtension(IDatabaseConfigurationProviders context)
      : base(context)
    {
      this.ConnectionString.ProviderName = "System.Data.OracleClient";
    }

    IDatabaseOracleConfiguration IDatabaseOracleConfiguration.WithConnectionString(
      string connectionString)
    {
      this.WithConnectionString(connectionString);
      return (IDatabaseOracleConfiguration) this;
    }

    IDatabaseOracleConfiguration IDatabaseOracleConfiguration.WithConnectionString(
      OracleConnectionStringBuilder builder)
    {
      this.WithConnectionString((DbConnectionStringBuilder) builder);
      return (IDatabaseOracleConfiguration) this;
    }

    IDatabaseConfigurationProperties IDatabaseOraclePackageConfiguration.AndPrefix(
      string prefix)
    {
      if (string.IsNullOrEmpty(prefix))
        throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, nameof (prefix));
      this.currentOraclePackageData.Prefix = prefix;
      return (IDatabaseConfigurationProperties) this;
    }

    public IDatabaseOraclePackageConfiguration WithPackageNamed(
      string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, nameof (name));
      this.EnsureOracleSettings();
      this.EnsureOracleConnectionData();
      OraclePackageData oraclePackageData = new OraclePackageData();
      oraclePackageData.Name = name;
      this.currentOraclePackageData = oraclePackageData;
      this.currentOracleConnectionData.Packages.Add(this.currentOraclePackageData);
      return (IDatabaseOraclePackageConfiguration) this;
    }

    private void EnsureOracleSettings()
    {
      this.currentOracleSettings = this.Builder.Get<OracleConnectionSettings>("oracleConnectionSettings");
      if (this.currentOracleSettings != null)
        return;
      this.currentOracleSettings = new OracleConnectionSettings();
      this.Builder.AddSection("oracleConnectionSettings", (ConfigurationSection) this.currentOracleSettings);
    }

    private void EnsureOracleConnectionData()
    {
      if (this.currentOracleConnectionData != null)
        return;
      OracleConnectionData oracleConnectionData = new OracleConnectionData();
      oracleConnectionData.Name = this.ConnectionString.Name;
      this.currentOracleConnectionData = oracleConnectionData;
      this.currentOracleSettings.OracleConnectionsData.Add(this.currentOracleConnectionData);
    }
  }
}
