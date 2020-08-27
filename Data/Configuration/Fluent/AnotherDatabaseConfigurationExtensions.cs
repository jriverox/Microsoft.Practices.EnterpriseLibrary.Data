// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent.AnotherDatabaseConfigurationExtensions
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using System;
using System.Data.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent
{
  internal class AnotherDatabaseConfigurationExtensions : DatabaseConfigurationExtension, IDatabaseAnotherDatabaseConfiguration, IDatabaseDefaultConnectionString, IDatabaseConfigurationProperties, IDatabaseConfigurationProviderEntry, IDatabaseConfiguration, IFluentInterface
  {
    public AnotherDatabaseConfigurationExtensions(
      IDatabaseConfigurationProviders context,
      string providerName)
      : base(context)
    {
      if (string.IsNullOrEmpty(providerName))
        throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, nameof (providerName));
      this.ConnectionString.ProviderName = providerName;
    }

    IDatabaseAnotherDatabaseConfiguration IDatabaseAnotherDatabaseConfiguration.WithConnectionString(
      DbConnectionStringBuilder builder)
    {
      this.WithConnectionString(builder);
      return (IDatabaseAnotherDatabaseConfiguration) this;
    }
  }
}
