// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.CommandAccessor`1
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public abstract class CommandAccessor<TResult> : DataAccessor<TResult>
  {
    private readonly IResultSetMapper<TResult> resultSetMapper;
    private readonly Database database;

    protected CommandAccessor(Database database, IRowMapper<TResult> rowMapper)
      : this(database, (IResultSetMapper<TResult>) new CommandAccessor<TResult>.DefaultResultSetMapper(rowMapper))
    {
      if (rowMapper == null)
        throw new ArgumentNullException(nameof (rowMapper));
    }

    protected CommandAccessor(Database database, IResultSetMapper<TResult> resultSetMapper)
    {
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      if (resultSetMapper == null)
        throw new ArgumentNullException(nameof (resultSetMapper));
      this.database = database;
      this.resultSetMapper = resultSetMapper;
    }

    protected Database Database
    {
      get
      {
        return this.database;
      }
    }

    protected IEnumerable<TResult> Execute(DbCommand command)
    {
      IDataReader reader = this.database.ExecuteReader(command);
      foreach (TResult map in this.resultSetMapper.MapSet(reader))
        yield return map;
    }

    protected IAsyncResult BeginExecute(
      DbCommand command,
      IParameterMapper parameterMapper,
      AsyncCallback callback,
      object state,
      object[] parameterValues)
    {
      parameterMapper.AssignParameters(command, parameterValues);
      return this.database.BeginExecuteReader(command, callback, state);
    }

    public override IEnumerable<TResult> EndExecute(IAsyncResult asyncResult)
    {
      this.GuardAsyncAllowed();
      return this.resultSetMapper.MapSet(this.database.EndExecuteReader(asyncResult));
    }

    protected void GuardAsyncAllowed()
    {
      if (!this.database.SupportsAsync)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AsyncOperationsNotSupported, (object) this.database.GetType().FullName));
    }

    private class DefaultResultSetMapper : IResultSetMapper<TResult>
    {
      private readonly IRowMapper<TResult> rowMapper;

      public DefaultResultSetMapper(IRowMapper<TResult> rowMapper)
      {
        this.rowMapper = rowMapper;
      }

      public IEnumerable<TResult> MapSet(IDataReader reader)
      {
        using (reader)
        {
          while (reader.Read())
            yield return this.rowMapper.MapRow((IDataRecord) reader);
        }
      }
    }
  }
}
