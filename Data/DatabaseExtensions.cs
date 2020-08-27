// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.DatabaseExtensions
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Collections.Generic;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public static class DatabaseExtensions
  {
    public static IEnumerable<TResult> ExecuteSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      params object[] parameterValues)
      where TResult : new()
    {
      return database.CreateSprocAccessor<TResult>(procedureName).Execute(parameterValues);
    }

    public static IEnumerable<TResult> ExecuteSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      params object[] parameterValues)
      where TResult : new()
    {
      return database.CreateSprocAccessor<TResult>(procedureName, parameterMapper).Execute(parameterValues);
    }

    public static IEnumerable<TResult> ExecuteSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IRowMapper<TResult> rowMapper,
      params object[] parameterValues)
      where TResult : new()
    {
      return database.CreateSprocAccessor<TResult>(procedureName, rowMapper).Execute(parameterValues);
    }

    public static IEnumerable<TResult> ExecuteSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      IRowMapper<TResult> rowMapper,
      params object[] parameterValues)
      where TResult : new()
    {
      return database.CreateSprocAccessor<TResult>(procedureName, parameterMapper, rowMapper).Execute(parameterValues);
    }

    public static IEnumerable<TResult> ExecuteSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IResultSetMapper<TResult> resultSetMapper,
      params object[] parameterValues)
      where TResult : new()
    {
      return database.CreateSprocAccessor<TResult>(procedureName, resultSetMapper).Execute(parameterValues);
    }

    public static IEnumerable<TResult> ExecuteSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      IResultSetMapper<TResult> resultSetMapper,
      params object[] parameterValues)
      where TResult : new()
    {
      return database.CreateSprocAccessor<TResult>(procedureName, parameterMapper, resultSetMapper).Execute(parameterValues);
    }

    public static DataAccessor<TResult> CreateSprocAccessor<TResult>(
      this Database database,
      string procedureName)
      where TResult : new()
    {
      IRowMapper<TResult> rowMapper = MapBuilder<TResult>.BuildAllProperties();
      return database.CreateSprocAccessor<TResult>(procedureName, rowMapper);
    }

    public static DataAccessor<TResult> CreateSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IParameterMapper parameterMapper)
      where TResult : new()
    {
      IRowMapper<TResult> rowMapper = MapBuilder<TResult>.BuildAllProperties();
      return database.CreateSprocAccessor<TResult>(procedureName, parameterMapper, rowMapper);
    }

    public static DataAccessor<TResult> CreateSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IRowMapper<TResult> rowMapper)
    {
      if (string.IsNullOrEmpty(procedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      return (DataAccessor<TResult>) new SprocAccessor<TResult>(database, procedureName, rowMapper);
    }

    public static DataAccessor<TResult> CreateSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      IRowMapper<TResult> rowMapper)
    {
      if (string.IsNullOrEmpty(procedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      return (DataAccessor<TResult>) new SprocAccessor<TResult>(database, procedureName, parameterMapper, rowMapper);
    }

    public static DataAccessor<TResult> CreateSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IResultSetMapper<TResult> resultSetMapper)
    {
      if (string.IsNullOrEmpty(procedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      return (DataAccessor<TResult>) new SprocAccessor<TResult>(database, procedureName, resultSetMapper);
    }

    public static DataAccessor<TResult> CreateSprocAccessor<TResult>(
      this Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      IResultSetMapper<TResult> resultSetMapper)
    {
      if (string.IsNullOrEmpty(procedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      return (DataAccessor<TResult>) new SprocAccessor<TResult>(database, procedureName, parameterMapper, resultSetMapper);
    }

    public static IEnumerable<TResult> ExecuteSqlStringAccessor<TResult>(
      this Database database,
      string sqlString)
      where TResult : new()
    {
      return database.CreateSqlStringAccessor<TResult>(sqlString).Execute();
    }

    public static IEnumerable<TResult> ExecuteSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IResultSetMapper<TResult> resultSetMapper)
    {
      return database.CreateSqlStringAccessor<TResult>(sqlString, resultSetMapper).Execute();
    }

    public static IEnumerable<TResult> ExecuteSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IRowMapper<TResult> rowMapper)
    {
      return database.CreateSqlStringAccessor<TResult>(sqlString, rowMapper).Execute();
    }

    public static DataAccessor<TResult> CreateSqlStringAccessor<TResult>(
      this Database database,
      string sqlString)
      where TResult : new()
    {
      IRowMapper<TResult> rowMapper = MapBuilder<TResult>.BuildAllProperties();
      return (DataAccessor<TResult>) new SqlStringAccessor<TResult>(database, sqlString, rowMapper);
    }

    public static DataAccessor<TResult> CreateSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IParameterMapper parameterMapper)
      where TResult : new()
    {
      IRowMapper<TResult> rowMapper = MapBuilder<TResult>.BuildAllProperties();
      return (DataAccessor<TResult>) new SqlStringAccessor<TResult>(database, sqlString, parameterMapper, rowMapper);
    }

    public static DataAccessor<TResult> CreateSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IRowMapper<TResult> rowMapper)
    {
      return (DataAccessor<TResult>) new SqlStringAccessor<TResult>(database, sqlString, rowMapper);
    }

    public static DataAccessor<TResult> CreateSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IResultSetMapper<TResult> resultSetMapper)
    {
      return (DataAccessor<TResult>) new SqlStringAccessor<TResult>(database, sqlString, resultSetMapper);
    }

    public static DataAccessor<TResult> CreateSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IParameterMapper parameterMapper,
      IRowMapper<TResult> rowMapper)
    {
      return (DataAccessor<TResult>) new SqlStringAccessor<TResult>(database, sqlString, parameterMapper, rowMapper);
    }

    public static DataAccessor<TResult> CreateSqlStringAccessor<TResult>(
      this Database database,
      string sqlString,
      IParameterMapper parameterMapper,
      IResultSetMapper<TResult> resultSetMapper)
    {
      return (DataAccessor<TResult>) new SqlStringAccessor<TResult>(database, sqlString, parameterMapper, resultSetMapper);
    }
  }
}
