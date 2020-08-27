// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent.SqlCeDatabaseConfigurationExtension
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent
{
  internal class SqlCeDatabaseConfigurationExtension : DatabaseConfigurationExtension, IDatabaseSqlCeDatabaseConfiguration, IDatabaseDefaultConnectionString, IDatabaseConfigurationProperties, IDatabaseConfigurationProviderEntry, IDatabaseConfiguration, IFluentInterface
  {
    public SqlCeDatabaseConfigurationExtension(IDatabaseConfigurationProviders context)
      : base(context)
    {
      this.ConnectionString.ProviderName = "System.Data.SqlServerCe";
    }
  }
}
