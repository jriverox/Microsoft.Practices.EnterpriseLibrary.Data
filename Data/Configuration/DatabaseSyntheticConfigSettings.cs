// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseSyntheticConfigSettings
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration
{
  public class DatabaseSyntheticConfigSettings : ITypeRegistrationsProvider
  {
    private static readonly DbProviderMapping defaultSqlMapping = new DbProviderMapping("System.Data.SqlClient", typeof (SqlDatabase));
    private static readonly DbProviderMapping defaultOracleMapping = new DbProviderMapping("System.Data.OracleClient", typeof (OracleDatabase));
    private static readonly DbProviderMapping defaultGenericMapping = new DbProviderMapping("generic", typeof (GenericDatabase));
    private IConfigurationSource configurationSource;

    public DatabaseSyntheticConfigSettings()
    {
    }

    public DatabaseSyntheticConfigSettings(IConfigurationSource configurationSource)
    {
      this.configurationSource = configurationSource;
    }

    public string DefaultDatabase
    {
      get
      {
        return !(this.configurationSource.GetSection("dataConfiguration") is DatabaseSettings section) ? string.Empty : section.DefaultDatabase;
      }
    }

    public IEnumerable<DatabaseData> Databases
    {
      get
      {
        DatabaseSettings databaseSettings = (DatabaseSettings) this.configurationSource.GetSection("dataConfiguration");
        foreach (ConnectionStringSettings connectionString in (ConfigurationElementCollection) this.GetConnectionStrings())
        {
          if (DatabaseSyntheticConfigSettings.IsValidProviderName(connectionString.ProviderName))
            yield return this.GetDatabaseData(connectionString, databaseSettings);
        }
      }
    }

    public ConnectionStringSettings GetConnectionStringSettings(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      ConnectionStringSettings connectionString = this.GetConnectionStrings()[name];
      DatabaseSyntheticConfigSettings.ValidateConnectionStringSettings(name, connectionString);
      return connectionString;
    }

    private static void ValidateConnectionStringSettings(
      string name,
      ConnectionStringSettings connectionStringSettings)
    {
      if (connectionStringSettings == null)
        throw new ConfigurationErrorsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionNoDatabaseDefined, (object) name));
      if (string.IsNullOrEmpty(connectionStringSettings.ProviderName))
        throw new ConfigurationErrorsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionNoProviderDefinedForConnectionString, (object) name));
    }

    private ConnectionStringSettingsCollection GetConnectionStrings()
    {
      return !(this.configurationSource.GetSection("connectionStrings") is ConnectionStringsSection section) ? ConfigurationManager.ConnectionStrings : section.ConnectionStrings;
    }

    private static bool IsValidProviderName(string providerName)
    {
      return DbProviderFactories.GetFactoryClasses().Rows.Find((object) providerName) != null;
    }

    private DatabaseData GetDatabaseData(
      ConnectionStringSettings connectionString,
      DatabaseSettings databaseSettings)
    {
      return DatabaseSyntheticConfigSettings.CreateDatabaseData(DatabaseSyntheticConfigSettings.GetAttribute(DatabaseSyntheticConfigSettings.GetProviderMapping(connectionString.ProviderName, databaseSettings).DatabaseType).ConfigurationType, connectionString, this.configurationSource);
    }

    private static ConfigurationElementTypeAttribute GetAttribute(
      Type databaseType)
    {
      ConfigurationElementTypeAttribute customAttribute = (ConfigurationElementTypeAttribute) Attribute.GetCustomAttribute((MemberInfo) databaseType, typeof (ConfigurationElementTypeAttribute), false);
      if (customAttribute == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionNoConfigurationElementTypeAttribute, (object) databaseType.Name));
      return customAttribute;
    }

    private static DatabaseData CreateDatabaseData(
      Type configurationElementType,
      ConnectionStringSettings settings,
      IConfigurationSource source)
    {
      object instance;
      try
      {
        instance = Activator.CreateInstance(configurationElementType, (object) settings, (object) source);
      }
      catch (MissingMethodException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionDatabaseDataTypeDoesNotHaveRequiredConstructor, (object) configurationElementType), (Exception) ex);
      }
      try
      {
        return (DatabaseData) instance;
      }
      catch (InvalidCastException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionDatabaseDataTypeDoesNotInheritFromDatabaseData, (object) configurationElementType), (Exception) ex);
      }
    }

    public DbProviderMapping GetProviderMapping(string dbProviderName)
    {
      DatabaseSettings section = (DatabaseSettings) this.configurationSource.GetSection("dataConfiguration");
      return DatabaseSyntheticConfigSettings.GetProviderMapping(dbProviderName, section);
    }

    private static DbProviderMapping GetProviderMapping(
      string dbProviderName,
      DatabaseSettings databaseSettings)
    {
      if (databaseSettings != null)
      {
        DbProviderMapping dbProviderMapping = databaseSettings.ProviderMappings.Get(dbProviderName);
        if (dbProviderMapping != null)
          return dbProviderMapping;
      }
      return DatabaseSyntheticConfigSettings.GetDefaultMapping(dbProviderName) ?? DatabaseSyntheticConfigSettings.GetGenericMapping();
    }

    private static DbProviderMapping GetDefaultMapping(string dbProviderName)
    {
      if ("System.Data.SqlClient".Equals(dbProviderName))
        return DatabaseSyntheticConfigSettings.defaultSqlMapping;
      if ("System.Data.OracleClient".Equals(dbProviderName))
        return DatabaseSyntheticConfigSettings.defaultOracleMapping;
      DbProviderFactory factory = DbProviderFactories.GetFactory(dbProviderName);
      if (SqlClientFactory.Instance == factory)
        return DatabaseSyntheticConfigSettings.defaultSqlMapping;
      return OracleClientFactory.Instance == factory ? DatabaseSyntheticConfigSettings.defaultOracleMapping : (DbProviderMapping) null;
    }

    private static DbProviderMapping GetGenericMapping()
    {
      return DatabaseSyntheticConfigSettings.defaultGenericMapping;
    }

    private static TypeRegistration GetInstrumentationProviderRegistration(
      string instanceName,
      IConfigurationSource configurationSource)
    {
      InstrumentationConfigurationSection instrumentationSection = InstrumentationConfigurationSection.GetSection(configurationSource);
      TypeRegistration<IDataInstrumentationProvider> typeRegistration = new TypeRegistration<IDataInstrumentationProvider>((Expression<Func<IDataInstrumentationProvider>>) (() => new NewDataInstrumentationProvider(instanceName, instrumentationSection.PerformanceCountersEnabled, instrumentationSection.EventLoggingEnabled, instrumentationSection.ApplicationInstanceName)));
      typeRegistration.Name = instanceName;
      return (TypeRegistration) typeRegistration;
    }

    private TypeRegistration GetDefaultDataEventLoggerRegistration()
    {
      InstrumentationConfigurationSection instrumentationConfigurationSection = InstrumentationConfigurationSection.GetSection(this.configurationSource);
      TypeRegistration<DefaultDataEventLogger> typeRegistration = new TypeRegistration<DefaultDataEventLogger>((Expression<Func<DefaultDataEventLogger>>) (() => new DefaultDataEventLogger(instrumentationConfigurationSection.EventLoggingEnabled)));
      typeRegistration.IsDefault = true;
      return (TypeRegistration) typeRegistration;
    }

    public IEnumerable<TypeRegistration> GetRegistrations(
      IConfigurationSource configurationSource)
    {
      if (configurationSource == null)
        throw new ArgumentNullException(nameof (configurationSource));
      this.configurationSource = configurationSource;
      return (IEnumerable<TypeRegistration>) this.DoGetRegistrations().Select<TypeRegistration, TypeRegistration>((Func<TypeRegistration, TypeRegistration>) (r => DatabaseSyntheticConfigSettings.MarkAsPublicName<Database>(r))).ToList<TypeRegistration>();
    }

    private static TypeRegistration MarkAsPublicName<TService>(
      TypeRegistration registration)
    {
      if (registration.ServiceType == typeof (TService))
        registration.IsPublicName = true;
      return registration;
    }

    private IEnumerable<TypeRegistration> DoGetRegistrations()
    {
      string defaultDatabase = this.DefaultDatabase;
      foreach (DatabaseData databaseData in this.Databases.ToList<DatabaseData>())
      {
        foreach (TypeRegistration typeRegistration in databaseData.GetRegistrations().ToList<TypeRegistration>())
        {
          if (typeRegistration.ServiceType == typeof (Database) && string.Equals(typeRegistration.Name, defaultDatabase))
            typeRegistration.IsDefault = true;
          yield return typeRegistration;
          yield return DatabaseSyntheticConfigSettings.GetInstrumentationProviderRegistration(typeRegistration.Name, this.configurationSource);
        }
      }
      yield return this.GetDefaultDataEventLoggerRegistration();
    }

    public IEnumerable<TypeRegistration> GetUpdatedRegistrations(
      IConfigurationSource configurationSource)
    {
      return this.GetRegistrations(configurationSource);
    }
  }
}
