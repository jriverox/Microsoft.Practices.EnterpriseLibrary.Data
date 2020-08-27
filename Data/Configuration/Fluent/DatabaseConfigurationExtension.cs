// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent.DatabaseConfigurationExtension
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using System;
using System.Configuration;
using System.Data.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent
{
  public abstract class DatabaseConfigurationExtension : IDatabaseConfigurationProperties, IDatabaseConfigurationProviderEntry, IDatabaseConfiguration, IFluentInterface
  {
    private readonly IDatabaseConfigurationProperties context;

    protected DatabaseConfigurationExtension(IDatabaseConfigurationProviders context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      this.context = (IDatabaseConfigurationProperties) context;
    }

    public IDatabaseConfigurationProviders ThatIs
    {
      get
      {
        return this.context.ThatIs;
      }
    }

    public IDatabaseConfigurationProperties ForDatabaseNamed(
      string databaseName)
    {
      return this.context.ForDatabaseNamed(databaseName);
    }

    public IDatabaseConfigurationProperties AsDefault()
    {
      return this.context.AsDefault();
    }

    public ConnectionStringSettings ConnectionString
    {
      get
      {
        return ((IDatabaseProviderExtensionContext) this.context).ConnectionString;
      }
    }

    public IConfigurationSourceBuilder Builder
    {
      get
      {
        return ((IDatabaseProviderExtensionContext) this.context).Builder;
      }
    }

    public IDatabaseConfigurationProperties WithConnectionString(
      string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, nameof (connectionString));
      this.ConnectionString.ConnectionString = connectionString;
      return (IDatabaseConfigurationProperties) this;
    }

    public IDatabaseConfigurationProperties WithConnectionString(
      DbConnectionStringBuilder builder)
    {
      if (builder == null)
        throw new ArgumentNullException(nameof (builder));
      return this.WithConnectionString(builder.ConnectionString);
    }

    Type IFluentInterface.GetType()
    {
      return this.GetType();
    }
  }
}
