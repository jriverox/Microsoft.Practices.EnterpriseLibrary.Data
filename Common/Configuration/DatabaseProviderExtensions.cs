﻿using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration
{
  public static class DatabaseProviderExtensions
  {
    public static IDatabaseSqlDatabaseConfiguration ASqlDatabase(
      this IDatabaseConfigurationProviders context)
    {
      return (IDatabaseSqlDatabaseConfiguration) new SqlDatabaseConfigurationExtension(context);
    }

    public static IDatabaseSqlCeDatabaseConfiguration ASqlCeDatabase(
      this IDatabaseConfigurationProviders context)
    {
      return (IDatabaseSqlCeDatabaseConfiguration) new SqlCeDatabaseConfigurationExtension(context);
    }

    public static IOleDbDatabaseConfiguration AnOleDbDatabase(
      this IDatabaseConfigurationProviders context)
    {
      return (IOleDbDatabaseConfiguration) new OleDbConfigurationExtension(context);
    }

    public static IOdbcDatabaseConfiguration AnOdbcDatabase(
      this IDatabaseConfigurationProviders context)
    {
      return (IOdbcDatabaseConfiguration) new OdbcConfigurationExtension(context);
    }

    public static IDatabaseOracleConfiguration AnOracleDatabase(
      this IDatabaseConfigurationProviders context)
    {
      return (IDatabaseOracleConfiguration) new OracleConfigurationExtension(context);
    }

    public static IDatabaseAnotherDatabaseConfiguration AnotherDatabaseType(
      this IDatabaseConfigurationProviders context,
      string providerName)
    {
      return (IDatabaseAnotherDatabaseConfiguration) new AnotherDatabaseConfigurationExtensions(context, providerName);
    }
  }
}
