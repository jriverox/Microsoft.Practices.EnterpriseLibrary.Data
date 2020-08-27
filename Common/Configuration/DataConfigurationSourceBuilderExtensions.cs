// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Common.Configuration.DataConfigurationSourceBuilderExtensions
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent;
using System;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration
{
  public static class DataConfigurationSourceBuilderExtensions
  {
    public static IDataConfiguration ConfigureData(
      this IConfigurationSourceBuilder configurationSourceBuilderRoot)
    {
      return (IDataConfiguration) new DataConfigurationSourceBuilderExtensions.DataConfigurationBuilder(configurationSourceBuilderRoot);
    }

    private class DataConfigurationBuilder : IDatabaseConfigurationProviders, IDatabaseProviderExtensionContext, IDatabaseProviderConfiguration, IDataConfiguration, IDatabaseConfigurationProperties, IDatabaseConfigurationProviderEntry, IDatabaseConfiguration, IFluentInterface
    {
      private readonly ConnectionStringsSection connectionStringSection = new ConnectionStringsSection();
      private ConnectionStringSettings currentDatabaseConnectionInfo;
      private DatabaseSettings currentDatabaseSection;
      private DbProviderMapping currentProviderMapping;

      private IConfigurationSourceBuilder Builder { get; set; }

      public DataConfigurationBuilder(IConfigurationSourceBuilder builder)
      {
        this.Builder = builder;
        builder.AddSection("connectionStrings", (ConfigurationSection) this.connectionStringSection);
      }

      public IDatabaseConfigurationProperties ForDatabaseNamed(
        string databaseName)
      {
        if (string.IsNullOrEmpty(databaseName))
          throw new ArgumentException(Microsoft.Practices.EnterpriseLibrary.Common.Properties.Resources.ExceptionStringNullOrEmpty, nameof (databaseName));
        this.ResetForNewDatabase(databaseName);
        this.connectionStringSection.ConnectionStrings.Add(this.currentDatabaseConnectionInfo);
        return (IDatabaseConfigurationProperties) this;
      }

      public IDatabaseConfigurationProperties AsDefault()
      {
        this.EnsureDatabaseSettings();
        this.currentDatabaseSection.DefaultDatabase = this.currentDatabaseConnectionInfo.Name;
        return (IDatabaseConfigurationProperties) this;
      }

      public IDatabaseConfigurationProviders ThatIs
      {
        get
        {
          return (IDatabaseConfigurationProviders) this;
        }
      }

      public IDatabaseProviderConfiguration WithProviderNamed(
        string providerName)
      {
        if (string.IsNullOrEmpty(providerName))
          throw new ArgumentException(Microsoft.Practices.EnterpriseLibrary.Common.Properties.Resources.ExceptionStringNullOrEmpty, nameof (providerName));
        this.EnsureDatabaseSettings();
        this.currentProviderMapping = new DbProviderMapping();
        this.currentProviderMapping.Name = providerName;
        this.currentProviderMapping.DatabaseType = typeof (GenericDatabase);
        this.currentDatabaseSection.ProviderMappings.Add(this.currentProviderMapping);
        return (IDatabaseProviderConfiguration) this;
      }

      public IDataConfiguration MappedToDatabase(Type databaseType)
      {
        if (!typeof (Database).IsAssignableFrom(databaseType))
          throw new ArgumentException(Microsoft.Practices.EnterpriseLibrary.Data.Properties.Resources.ExceptionArgumentMustInheritFromDatabase, nameof (databaseType));
        this.currentProviderMapping.DatabaseType = databaseType;
        return (IDataConfiguration) this;
      }

      public IDataConfiguration MappedToDatabase<T>() where T : Database
      {
        return this.MappedToDatabase(typeof (T));
      }

      ConnectionStringSettings IDatabaseProviderExtensionContext.ConnectionString
      {
        get
        {
          return this.currentDatabaseConnectionInfo;
        }
      }

      IConfigurationSourceBuilder IDatabaseProviderExtensionContext.Builder
      {
        get
        {
          return this.Builder;
        }
      }

      private void ResetForNewDatabase(string databaseName)
      {
        this.currentDatabaseConnectionInfo = new ConnectionStringSettings()
        {
          Name = databaseName,
          ProviderName = "System.Data.SqlClient",
          ConnectionString = Microsoft.Practices.EnterpriseLibrary.Data.Properties.Resources.DefaultSqlConnctionString
        };
      }

      private void EnsureDatabaseSettings()
      {
        if (this.currentDatabaseSection != null)
          return;
        this.currentDatabaseSection = new DatabaseSettings();
        this.Builder.AddSection("dataConfiguration", (ConfigurationSection) this.currentDatabaseSection);
      }

      Type IFluentInterface.GetType()
      {
        return this.GetType();
      }
    }
  }
}
