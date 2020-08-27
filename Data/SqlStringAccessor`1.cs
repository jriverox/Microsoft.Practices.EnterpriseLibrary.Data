// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.SqlStringAccessor`1
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class SqlStringAccessor<TResult> : CommandAccessor<TResult>
  {
    private readonly IParameterMapper parameterMapper;
    private readonly string sqlString;

    public SqlStringAccessor(Database database, string sqlString, IRowMapper<TResult> rowMapper)
      : this(database, sqlString, (IParameterMapper) new SqlStringAccessor<TResult>.DefaultSqlStringParameterMapper(), rowMapper)
    {
    }

    public SqlStringAccessor(
      Database database,
      string sqlString,
      IResultSetMapper<TResult> resultSetMapper)
      : this(database, sqlString, (IParameterMapper) new SqlStringAccessor<TResult>.DefaultSqlStringParameterMapper(), resultSetMapper)
    {
    }

    public SqlStringAccessor(
      Database database,
      string sqlString,
      IParameterMapper parameterMapper,
      IRowMapper<TResult> rowMapper)
      : base(database, rowMapper)
    {
      if (string.IsNullOrEmpty(sqlString))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      if (parameterMapper == null)
        throw new ArgumentNullException(nameof (parameterMapper));
      this.parameterMapper = parameterMapper;
      this.sqlString = sqlString;
    }

    public SqlStringAccessor(
      Database database,
      string sqlString,
      IParameterMapper parameterMapper,
      IResultSetMapper<TResult> resultSetMapper)
      : base(database, resultSetMapper)
    {
      if (string.IsNullOrEmpty(sqlString))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      if (parameterMapper == null)
        throw new ArgumentNullException(nameof (parameterMapper));
      this.parameterMapper = parameterMapper;
      this.sqlString = sqlString;
    }

    public override IEnumerable<TResult> Execute(params object[] parameterValues)
    {
      using (DbCommand command = this.Database.GetSqlStringCommand(this.sqlString))
      {
        this.parameterMapper.AssignParameters(command, parameterValues);
        foreach (TResult result in this.Execute(command))
          yield return result;
      }
    }

    public override IAsyncResult BeginExecute(
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      this.GuardAsyncAllowed();
      using (DbCommand sqlStringCommand = this.Database.GetSqlStringCommand(this.sqlString))
        return this.BeginExecute(sqlStringCommand, this.parameterMapper, callback, state, parameterValues);
    }

    private class DefaultSqlStringParameterMapper : IParameterMapper
    {
      public void AssignParameters(DbCommand command, object[] parameterValues)
      {
        if (parameterValues.Length > 0)
          throw new InvalidOperationException(Resources.ExceptionSqlStringAccessorCannotDiscoverParameters);
      }
    }
  }
}
