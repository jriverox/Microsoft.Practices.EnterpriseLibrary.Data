// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql.Configuration;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Permissions;
using System.Xml;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Sql
{
  [ConfigurationElementType(typeof (SqlDatabaseData))]
  [SqlClientPermission(SecurityAction.Demand)]
  public class SqlDatabase : Database
  {
    public SqlDatabase(string connectionString)
      : base(connectionString, (DbProviderFactory) SqlClientFactory.Instance)
    {
    }

    public SqlDatabase(
      string connectionString,
      IDataInstrumentationProvider instrumentationProvider)
      : base(connectionString, (DbProviderFactory) SqlClientFactory.Instance, instrumentationProvider)
    {
    }

    protected char ParameterToken
    {
      get
      {
        return '@';
      }
    }

    public override bool SupportsAsync
    {
      get
      {
        return true;
      }
    }

    public XmlReader ExecuteXmlReader(DbCommand command)
    {
      SqlCommand sqlCommand = SqlDatabase.CheckIfSqlCommand(command);
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        Database.PrepareCommand(command, openConnection.Connection);
        return (XmlReader) new RefCountingXmlReader(openConnection, this.DoExecuteXmlReader(sqlCommand));
      }
    }

    public XmlReader ExecuteXmlReader(DbCommand command, DbTransaction transaction)
    {
      SqlCommand sqlCommand = SqlDatabase.CheckIfSqlCommand(command);
      Database.PrepareCommand((DbCommand) sqlCommand, transaction);
      return this.DoExecuteXmlReader(sqlCommand);
    }

    public IAsyncResult BeginExecuteXmlReader(
      DbCommand command,
      AsyncCallback callback,
      object state)
    {
      SqlCommand command1 = SqlDatabase.CheckIfSqlCommand(command);
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand(command, newOpenConnection);
        return this.DoBeginExecuteXmlReader(command1, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public IAsyncResult BeginExecuteXmlReader(
      DbCommand command,
      DbTransaction transaction,
      AsyncCallback callback,
      object state)
    {
      SqlCommand command1 = SqlDatabase.CheckIfSqlCommand(command);
      Database.PrepareCommand((DbCommand) command1, transaction);
      return this.DoBeginExecuteXmlReader(command1, callback, state);
    }

    public XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
    {
      DaabAsyncResult daabAsyncResult = (DaabAsyncResult) asyncResult;
      SqlCommand command = (SqlCommand) daabAsyncResult.Command;
      try
      {
        XmlReader innerReader = command.EndExecuteXmlReader(daabAsyncResult.InnerAsyncResult);
        this.instrumentationProvider.FireCommandExecutedEvent(daabAsyncResult.StartTime);
        if (command.Transaction != null)
          return innerReader;
        using (DatabaseConnectionWrapper connection = new DatabaseConnectionWrapper((DbConnection) command.Connection))
          return (XmlReader) new RefCountingXmlReader(connection, innerReader);
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        if (command.Transaction == null)
          command.Connection.Close();
        throw;
      }
      finally
      {
        SqlDatabase.CleanupConnectionFromAsyncOperation(daabAsyncResult);
      }
    }

    private IAsyncResult DoBeginExecuteXmlReader(
      SqlCommand command,
      AsyncCallback callback,
      object state)
    {
      try
      {
        return WrappedAsyncOperation.BeginAsyncOperation<DaabAsyncResult>(callback, (Func<AsyncCallback, IAsyncResult>) (cb => command.BeginExecuteXmlReader(cb, state)), (Func<IAsyncResult, DaabAsyncResult>) (ar => new DaabAsyncResult(ar, (DbCommand) command, false, false, DateTime.Now)));
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    private XmlReader DoExecuteXmlReader(SqlCommand sqlCommand)
    {
      try
      {
        DateTime now = DateTime.Now;
        XmlReader xmlReader = sqlCommand.ExecuteXmlReader();
        this.instrumentationProvider.FireCommandExecutedEvent(now);
        return xmlReader;
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(sqlCommand.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    private static SqlCommand CheckIfSqlCommand(DbCommand command)
    {
      if (!(command is SqlCommand sqlCommand))
        throw new ArgumentException(Resources.ExceptionCommandNotSqlCommand, nameof (command));
      return sqlCommand;
    }

    private void OnSqlRowUpdated(object sender, SqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
    {
      if (rowThatCouldNotBeWritten.RecordsAffected != 0 || rowThatCouldNotBeWritten.Errors == null)
        return;
      rowThatCouldNotBeWritten.Row.RowError = Resources.ExceptionMessageUpdateDataSetRowFailure;
      rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
    }

    public override bool SupportsParemeterDiscovery
    {
      get
      {
        return true;
      }
    }

    protected override void DeriveParameters(DbCommand discoveryCommand)
    {
      SqlCommandBuilder.DeriveParameters((SqlCommand) discoveryCommand);
    }

    protected override int UserParametersStartIndex()
    {
      return 1;
    }

    public override string BuildParameterName(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      return (int) name[0] != (int) this.ParameterToken ? name.Insert(0, new string(this.ParameterToken, 1)) : name;
    }

    protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter)
    {
      ((SqlDataAdapter) adapter).RowUpdated += new SqlRowUpdatedEventHandler(this.OnSqlRowUpdated);
    }

    protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
    {
      int num = 1;
      return command.Parameters.Count - num == values.Length;
    }

    public virtual void AddParameter(
      DbCommand command,
      string name,
      SqlDbType dbType,
      int size,
      ParameterDirection direction,
      bool nullable,
      byte precision,
      byte scale,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      DbParameter parameter = this.CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
      command.Parameters.Add((object) parameter);
    }

    public void AddParameter(
      DbCommand command,
      string name,
      SqlDbType dbType,
      ParameterDirection direction,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      this.AddParameter(command, name, dbType, 0, direction, false, (byte) 0, (byte) 0, sourceColumn, sourceVersion, value);
    }

    public void AddOutParameter(DbCommand command, string name, SqlDbType dbType, int size)
    {
      this.AddParameter(command, name, dbType, size, ParameterDirection.Output, true, (byte) 0, (byte) 0, string.Empty, DataRowVersion.Default, (object) DBNull.Value);
    }

    public void AddInParameter(DbCommand command, string name, SqlDbType dbType)
    {
      this.AddParameter(command, name, dbType, ParameterDirection.Input, string.Empty, DataRowVersion.Default, (object) null);
    }

    public void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value)
    {
      this.AddParameter(command, name, dbType, ParameterDirection.Input, string.Empty, DataRowVersion.Default, value);
    }

    public void AddInParameter(
      DbCommand command,
      string name,
      SqlDbType dbType,
      string sourceColumn,
      DataRowVersion sourceVersion)
    {
      this.AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, (byte) 0, (byte) 0, sourceColumn, sourceVersion, (object) null);
    }

    protected DbParameter CreateParameter(
      string name,
      SqlDbType dbType,
      int size,
      ParameterDirection direction,
      bool nullable,
      byte precision,
      byte scale,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      SqlParameter parameter = this.CreateParameter(name) as SqlParameter;
      this.ConfigureParameter(parameter, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
      return (DbParameter) parameter;
    }

    protected virtual void ConfigureParameter(
      SqlParameter param,
      string name,
      SqlDbType dbType,
      int size,
      ParameterDirection direction,
      bool nullable,
      byte precision,
      byte scale,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      param.SqlDbType = dbType;
      param.Size = size;
      param.Value = value ?? (object) DBNull.Value;
      param.Direction = direction;
      param.IsNullable = nullable;
      param.SourceColumn = sourceColumn;
      param.SourceVersion = sourceVersion;
    }

    private static SqlCommand CreateSqlCommandByCommandType(
      CommandType commandType,
      string commandText)
    {
      SqlCommand sqlCommand = new SqlCommand(commandText);
      sqlCommand.CommandType = commandType;
      return sqlCommand;
    }

    private IAsyncResult DoBeginExecuteNonQuery(
      SqlCommand command,
      bool disposeCommand,
      AsyncCallback callback,
      object state)
    {
      bool closeConnection = command.Transaction == null;
      try
      {
        return WrappedAsyncOperation.BeginAsyncOperation<DaabAsyncResult>(callback, (Func<AsyncCallback, IAsyncResult>) (cb => command.BeginExecuteNonQuery(cb, state)), (Func<IAsyncResult, DaabAsyncResult>) (ar => new DaabAsyncResult(ar, (DbCommand) command, disposeCommand, closeConnection, DateTime.Now)));
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    public override IAsyncResult BeginExecuteNonQuery(
      DbCommand command,
      AsyncCallback callback,
      object state)
    {
      SqlCommand command1 = SqlDatabase.CheckIfSqlCommand(command);
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand((DbCommand) command1, newOpenConnection);
        return this.DoBeginExecuteNonQuery(command1, false, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public override IAsyncResult BeginExecuteNonQuery(
      DbCommand command,
      DbTransaction transaction,
      AsyncCallback callback,
      object state)
    {
      SqlCommand command1 = SqlDatabase.CheckIfSqlCommand(command);
      Database.PrepareCommand((DbCommand) command1, transaction);
      return this.DoBeginExecuteNonQuery(command1, false, callback, state);
    }

    public override IAsyncResult BeginExecuteNonQuery(
      string storedProcedureName,
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      SqlCommand command = SqlDatabase.CheckIfSqlCommand(this.GetStoredProcCommand(storedProcedureName, parameterValues));
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand((DbCommand) command, newOpenConnection);
        return this.DoBeginExecuteNonQuery(command, true, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public override IAsyncResult BeginExecuteNonQuery(
      DbTransaction transaction,
      string storedProcedureName,
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      SqlCommand command = SqlDatabase.CheckIfSqlCommand(this.GetStoredProcCommand(storedProcedureName, parameterValues));
      Database.PrepareCommand((DbCommand) command, transaction);
      return this.DoBeginExecuteNonQuery(command, true, callback, state);
    }

    public override IAsyncResult BeginExecuteNonQuery(
      CommandType commandType,
      string commandText,
      AsyncCallback callback,
      object state)
    {
      SqlCommand commandByCommandType = SqlDatabase.CreateSqlCommandByCommandType(commandType, commandText);
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand((DbCommand) commandByCommandType, newOpenConnection);
        return this.DoBeginExecuteNonQuery(commandByCommandType, true, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public override IAsyncResult BeginExecuteNonQuery(
      DbTransaction transaction,
      CommandType commandType,
      string commandText,
      AsyncCallback callback,
      object state)
    {
      SqlCommand commandByCommandType = SqlDatabase.CreateSqlCommandByCommandType(commandType, commandText);
      Database.PrepareCommand((DbCommand) commandByCommandType, transaction);
      return this.DoBeginExecuteNonQuery(commandByCommandType, true, callback, state);
    }

    public override int EndExecuteNonQuery(IAsyncResult asyncResult)
    {
      DaabAsyncResult daabAsyncResult = (DaabAsyncResult) asyncResult;
      SqlCommand command = (SqlCommand) daabAsyncResult.Command;
      try
      {
        int num = command.EndExecuteNonQuery(daabAsyncResult.InnerAsyncResult);
        this.instrumentationProvider.FireCommandExecutedEvent(daabAsyncResult.StartTime);
        return num;
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
      finally
      {
        SqlDatabase.CleanupConnectionFromAsyncOperation(daabAsyncResult);
      }
    }

    private IAsyncResult DoBeginExecuteReader(
      SqlCommand command,
      bool disposeCommand,
      AsyncCallback callback,
      object state)
    {
      CommandBehavior commandBehavior = command.Transaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default;
      try
      {
        return WrappedAsyncOperation.BeginAsyncOperation<DaabAsyncResult>(callback, (Func<AsyncCallback, IAsyncResult>) (cb => command.BeginExecuteReader(cb, state, commandBehavior)), (Func<IAsyncResult, DaabAsyncResult>) (ar => new DaabAsyncResult(ar, (DbCommand) command, disposeCommand, false, DateTime.Now)));
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    public override IAsyncResult BeginExecuteReader(
      DbCommand command,
      AsyncCallback callback,
      object state)
    {
      SqlCommand command1 = SqlDatabase.CheckIfSqlCommand(command);
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand((DbCommand) command1, newOpenConnection);
        return this.DoBeginExecuteReader(command1, false, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public override IAsyncResult BeginExecuteReader(
      DbCommand command,
      DbTransaction transaction,
      AsyncCallback callback,
      object state)
    {
      SqlCommand command1 = SqlDatabase.CheckIfSqlCommand(command);
      Database.PrepareCommand((DbCommand) command1, transaction);
      return this.DoBeginExecuteReader(command1, false, callback, state);
    }

    public override IAsyncResult BeginExecuteReader(
      string storedProcedureName,
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      SqlCommand command = SqlDatabase.CheckIfSqlCommand(this.GetStoredProcCommand(storedProcedureName, parameterValues));
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand((DbCommand) command, newOpenConnection);
        return this.DoBeginExecuteReader(command, true, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public override IAsyncResult BeginExecuteReader(
      DbTransaction transaction,
      string storedProcedureName,
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      SqlCommand command = SqlDatabase.CheckIfSqlCommand(this.GetStoredProcCommand(storedProcedureName, parameterValues));
      Database.PrepareCommand((DbCommand) command, transaction);
      return this.DoBeginExecuteReader(command, true, callback, state);
    }

    public override IAsyncResult BeginExecuteReader(
      CommandType commandType,
      string commandText,
      AsyncCallback callback,
      object state)
    {
      SqlCommand commandByCommandType = SqlDatabase.CreateSqlCommandByCommandType(commandType, commandText);
      DbConnection newOpenConnection = this.GetNewOpenConnection();
      try
      {
        Database.PrepareCommand((DbCommand) commandByCommandType, newOpenConnection);
        return this.DoBeginExecuteReader(commandByCommandType, true, callback, state);
      }
      catch
      {
        newOpenConnection.Close();
        throw;
      }
    }

    public override IAsyncResult BeginExecuteReader(
      DbTransaction transaction,
      CommandType commandType,
      string commandText,
      AsyncCallback callback,
      object state)
    {
      SqlCommand commandByCommandType = SqlDatabase.CreateSqlCommandByCommandType(commandType, commandText);
      Database.PrepareCommand((DbCommand) commandByCommandType, transaction);
      return this.DoBeginExecuteReader(commandByCommandType, true, callback, state);
    }

    public override IDataReader EndExecuteReader(IAsyncResult asyncResult)
    {
      DaabAsyncResult daabAsyncResult = (DaabAsyncResult) asyncResult;
      SqlCommand command = (SqlCommand) daabAsyncResult.Command;
      try
      {
        IDataReader dataReader = (IDataReader) command.EndExecuteReader(daabAsyncResult.InnerAsyncResult);
        this.instrumentationProvider.FireCommandExecutedEvent(daabAsyncResult.StartTime);
        return dataReader;
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        if (command.Transaction == null)
          command.Connection.Close();
        throw;
      }
      finally
      {
        SqlDatabase.CleanupConnectionFromAsyncOperation(daabAsyncResult);
      }
    }

    public override IAsyncResult BeginExecuteScalar(
      DbCommand command,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExecuteReader(command, callback, state);
    }

    public override IAsyncResult BeginExecuteScalar(
      DbCommand command,
      DbTransaction transaction,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExecuteReader(command, transaction, callback, state);
    }

    public override IAsyncResult BeginExecuteScalar(
      string storedProcedureName,
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      return this.BeginExecuteReader(storedProcedureName, callback, state, parameterValues);
    }

    public override IAsyncResult BeginExecuteScalar(
      DbTransaction transaction,
      string storedProcedureName,
      AsyncCallback callback,
      object state,
      params object[] parameterValues)
    {
      return this.BeginExecuteReader(transaction, storedProcedureName, callback, state, parameterValues);
    }

    public override IAsyncResult BeginExecuteScalar(
      CommandType commandType,
      string commandText,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExecuteReader(commandType, commandText, callback, state);
    }

    public override IAsyncResult BeginExecuteScalar(
      DbTransaction transaction,
      CommandType commandType,
      string commandText,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExecuteReader(transaction, commandType, commandText, callback, state);
    }

    public override object EndExecuteScalar(IAsyncResult asyncResult)
    {
      using (IDataReader dataReader = this.EndExecuteReader(asyncResult))
        return !dataReader.Read() || dataReader.FieldCount == 0 ? (object) null : dataReader.GetValue(0);
    }

    private static void CleanupConnectionFromAsyncOperation(DaabAsyncResult daabAsyncResult)
    {
      if (daabAsyncResult.DisposeCommand && daabAsyncResult.Command != null)
        daabAsyncResult.Command.Dispose();
      if (!daabAsyncResult.CloseConnection || daabAsyncResult.Connection == null)
        return;
      daabAsyncResult.Connection.Close();
    }
  }
}
