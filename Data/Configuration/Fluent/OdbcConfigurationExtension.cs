// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent.OdbcConfigurationExtension
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Data.Common;
using System.Data.Odbc;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent
{
  internal class OdbcConfigurationExtension : DatabaseConfigurationExtension, IOdbcDatabaseConfiguration, IDatabaseDefaultConnectionString, IDatabaseConfigurationProperties, IDatabaseConfigurationProviderEntry, IDatabaseConfiguration, IFluentInterface
  {
    public OdbcConfigurationExtension(IDatabaseConfigurationProviders context)
      : base(context)
    {
      this.ConnectionString.ProviderName = "System.Data.Odbc";
    }

    public IDatabaseConfigurationProperties WithConnectionString(
      OdbcConnectionStringBuilder builder)
    {
      return this.WithConnectionString((DbConnectionStringBuilder) builder);
    }
  }
}
