// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Database
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;
using System.Transactions;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public abstract class Database
  {
    private static readonly ParameterCache parameterCache = new ParameterCache();
    private static readonly string VALID_PASSWORD_TOKENS = Resources.Password;
    private static readonly string VALID_USER_ID_TOKENS = Resources.UserName;
    private readonly Microsoft.Practices.EnterpriseLibrary.Data.ConnectionString connectionString;
    private readonly DbProviderFactory dbProviderFactory;
    protected IDataInstrumentationProvider instrumentationProvider;

    protected Database(string connectionString, DbProviderFactory dbProviderFactory)
      : this(connectionString, dbProviderFactory, (IDataInstrumentationProvider) new NullDataInstrumentationProvider())
    {
    }

    protected Database(
      string connectionString,
      DbProviderFactory dbProviderFactory,
      IDataInstrumentationProvider instrumentationProvider)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (connectionString));
      if (dbProviderFactory == null)
        throw new ArgumentNullException(nameof (dbProviderFactory));
      if (instrumentationProvider == null)
        throw new ArgumentNullException(nameof (instrumentationProvider));
      this.connectionString = new Microsoft.Practices.EnterpriseLibrary.Data.ConnectionString(connectionString, Database.VALID_USER_ID_TOKENS, Database.VALID_PASSWORD_TOKENS);
      this.dbProviderFactory = dbProviderFactory;
      this.instrumentationProvider = instrumentationProvider;
    }

    public string ConnectionString
    {
      get
      {
        return this.connectionString.ToString();
      }
    }

    protected string ConnectionStringNoCredentials
    {
      get
      {
        return this.connectionString.ToStringNoCredentials();
      }
    }

    public string ConnectionStringWithoutCredentials
    {
      get
      {
        return this.ConnectionStringNoCredentials;
      }
    }

    public DbProviderFactory DbProviderFactory
    {
      get
      {
        return this.dbProviderFactory;
      }
    }

    public void AddInParameter(DbCommand command, string name, DbType dbType)
    {
      this.AddParameter(command, name, dbType, ParameterDirection.Input, string.Empty, DataRowVersion.Default, (object) null);
    }

    public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
    {
      this.AddParameter(command, name, dbType, ParameterDirection.Input, string.Empty, DataRowVersion.Default, value);
    }

    public void AddInParameter(
      DbCommand command,
      string name,
      DbType dbType,
      string sourceColumn,
      DataRowVersion sourceVersion)
    {
      this.AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, (byte) 0, (byte) 0, sourceColumn, sourceVersion, (object) null);
    }

    public void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
    {
      this.AddParameter(command, name, dbType, size, ParameterDirection.Output, true, (byte) 0, (byte) 0, string.Empty, DataRowVersion.Default, (object) DBNull.Value);
    }

    public virtual void AddParameter(
      DbCommand command,
      string name,
      DbType dbType,
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
      DbType dbType,
      ParameterDirection direction,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      this.AddParameter(command, name, dbType, 0, direction, false, (byte) 0, (byte) 0, sourceColumn, sourceVersion, value);
    }

    private void AssignParameterValues(DbCommand command, object[] values)
    {
      int num = this.UserParametersStartIndex();
      for (int index = 0; index < values.Length; ++index)
      {
        IDataParameter parameter = (IDataParameter) command.Parameters[index + num];
        this.SetParameterValue(command, parameter.ParameterName, values[index]);
      }
    }

    private static DbTransaction BeginTransaction(DbConnection connection)
    {
      return connection.BeginTransaction();
    }

    public virtual string BuildParameterName(string name)
    {
      return name;
    }

    public static void ClearParameterCache()
    {
      Database.parameterCache.Clear();
    }

    private static void CommitTransaction(IDbTransaction tran)
    {
      tran.Commit();
    }

    protected virtual void ConfigureParameter(
      DbParameter param,
      string name,
      DbType dbType,
      int size,
      ParameterDirection direction,
      bool nullable,
      byte precision,
      byte scale,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      param.DbType = dbType;
      param.Size = size;
      param.Value = value ?? (object) DBNull.Value;
      param.Direction = direction;
      param.IsNullable = nullable;
      param.SourceColumn = sourceColumn;
      param.SourceVersion = sourceVersion;
    }

    private DbCommand CreateCommandByCommandType(
      CommandType commandType,
      string commandText)
    {
      DbCommand command = this.dbProviderFactory.CreateCommand();
      command.CommandType = commandType;
      command.CommandText = commandText;
      return command;
    }

    public virtual DbConnection CreateConnection()
    {
      DbConnection connection = this.dbProviderFactory.CreateConnection();
      connection.ConnectionString = this.ConnectionString;
      return connection;
    }

    protected DbParameter CreateParameter(
      string name,
      DbType dbType,
      int size,
      ParameterDirection direction,
      bool nullable,
      byte precision,
      byte scale,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      DbParameter parameter = this.CreateParameter(name);
      this.ConfigureParameter(parameter, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
      return parameter;
    }

    protected DbParameter CreateParameter(string name)
    {
      DbParameter parameter = this.dbProviderFactory.CreateParameter();
      parameter.ParameterName = this.BuildParameterName(name);
      return parameter;
    }

    public virtual bool SupportsParemeterDiscovery
    {
      get
      {
        return false;
      }
    }

    protected abstract void DeriveParameters(DbCommand discoveryCommand);

    public void DiscoverParameters(DbCommand command)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(command.CommandType, command.CommandText))
        {
          commandByCommandType.Connection = openConnection.Connection;
          this.DeriveParameters(commandByCommandType);
          foreach (ICloneable parameter in commandByCommandType.Parameters)
          {
            IDataParameter dataParameter = (IDataParameter) parameter.Clone();
            command.Parameters.Add((object) dataParameter);
          }
        }
      }
    }

    protected int DoExecuteNonQuery(DbCommand command)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      try
      {
        DateTime now = DateTime.Now;
        int num = command.ExecuteNonQuery();
        this.instrumentationProvider.FireCommandExecutedEvent(now);
        return num;
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    protected async Task<int> DoExecuteNonQueryAsync(DbCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));
        try
        {
            DateTime now = DateTime.Now;
            int num = await command.ExecuteNonQueryAsync();
            this.instrumentationProvider.FireCommandExecutedEvent(now);
            return num;
        }
        catch (Exception ex)
        {
            this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
            throw;
        }
    }

    private IDataReader DoExecuteReader(DbCommand command, CommandBehavior cmdBehavior)
    {
      try
      {
        DateTime now = DateTime.Now;
        IDataReader dataReader = (IDataReader) command.ExecuteReader(cmdBehavior);
        this.instrumentationProvider.FireCommandExecutedEvent(now);
        return dataReader;
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    private async Task<IDataReader> DoExecuteReaderAsync(DbCommand command, CommandBehavior cmdBehavior)
    {
        try
        {
            DateTime now = DateTime.Now;
            IDataReader dataReader = (IDataReader) await command.ExecuteReaderAsync(cmdBehavior);
            this.instrumentationProvider.FireCommandExecutedEvent(now);
            return dataReader;
        }
        catch (Exception ex)
        {
            this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
            throw;
        }
    }

    private object DoExecuteScalar(IDbCommand command)
    {
      try
      {
        DateTime now = DateTime.Now;
        object obj = command.ExecuteScalar();
        this.instrumentationProvider.FireCommandExecutedEvent(now);
        return obj;
      }
      catch (Exception ex)
      {
        this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
        throw;
      }
    }

    private void DoLoadDataSet(IDbCommand command, DataSet dataSet, string[] tableNames)
    {
      if (tableNames == null)
        throw new ArgumentNullException(nameof (tableNames));
      if (tableNames.Length == 0)
        throw new ArgumentException(Resources.ExceptionTableNameArrayEmpty, nameof (tableNames));
      for (int index = 0; index < tableNames.Length; ++index)
      {
        if (string.IsNullOrEmpty(tableNames[index]))
          throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "tableNames[" + (object) index + "]");
      }
      using (DbDataAdapter dataAdapter = this.GetDataAdapter(UpdateBehavior.Standard))
      {
        ((IDbDataAdapter) dataAdapter).SelectCommand = command;
        try
        {
          DateTime now = DateTime.Now;
          string str = "Table";
          for (int index = 0; index < tableNames.Length; ++index)
          {
            string sourceTable = index == 0 ? str : str + (object) index;
            dataAdapter.TableMappings.Add(sourceTable, tableNames[index]);
          }
          dataAdapter.Fill(dataSet);
          this.instrumentationProvider.FireCommandExecutedEvent(now);
        }
        catch (Exception ex)
        {
          this.instrumentationProvider.FireCommandFailedEvent(command.CommandText, this.ConnectionStringNoCredentials, ex);
          throw;
        }
      }
    }

    private int DoUpdateDataSet(
      UpdateBehavior behavior,
      DataSet dataSet,
      string tableName,
      IDbCommand insertCommand,
      IDbCommand updateCommand,
      IDbCommand deleteCommand,
      int? updateBatchSize)
    {
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (tableName));
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      if (insertCommand == null && updateCommand == null && deleteCommand == null)
        throw new ArgumentException(Resources.ExceptionMessageUpdateDataSetArgumentFailure);
      using (DbDataAdapter dataAdapter = this.GetDataAdapter(behavior))
      {
        IDbDataAdapter dbDataAdapter = (IDbDataAdapter) dataAdapter;
        if (insertCommand != null)
          dbDataAdapter.InsertCommand = insertCommand;
        if (updateCommand != null)
          dbDataAdapter.UpdateCommand = updateCommand;
        if (deleteCommand != null)
          dbDataAdapter.DeleteCommand = deleteCommand;
        if (updateBatchSize.HasValue)
        {
          dataAdapter.UpdateBatchSize = updateBatchSize.Value;
          if (insertCommand != null)
            dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
          if (updateCommand != null)
            dataAdapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
          if (deleteCommand != null)
            dataAdapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;
        }
        try
        {
          DateTime now = DateTime.Now;
          int num = dataAdapter.Update(dataSet.Tables[tableName]);
          this.instrumentationProvider.FireCommandExecutedEvent(now);
          return num;
        }
        catch (Exception ex)
        {
          this.instrumentationProvider.FireCommandFailedEvent("DbDataAdapter.Update() " + tableName, this.ConnectionStringNoCredentials, ex);
          throw;
        }
      }
    }

    public virtual DataSet ExecuteDataSet(DbCommand command)
    {
      DataSet dataSet = new DataSet();
      dataSet.Locale = CultureInfo.InvariantCulture;
      this.LoadDataSet(command, dataSet, "Table");
      return dataSet;
    }

    public virtual DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
    {
      DataSet dataSet = new DataSet();
      dataSet.Locale = CultureInfo.InvariantCulture;
      this.LoadDataSet(command, dataSet, "Table", transaction);
      return dataSet;
    }

    public virtual DataSet ExecuteDataSet(
      string storedProcedureName,
      params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteDataSet(storedProcCommand);
    }

    public virtual DataSet ExecuteDataSet(
      DbTransaction transaction,
      string storedProcedureName,
      params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteDataSet(storedProcCommand, transaction);
    }

    public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteDataSet(commandByCommandType);
    }

    public virtual DataSet ExecuteDataSet(
      DbTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteDataSet(commandByCommandType, transaction);
    }

    public virtual int ExecuteNonQuery(DbCommand command)
    {
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        Database.PrepareCommand(command, openConnection.Connection);
        return this.DoExecuteNonQuery(command);
      }
    }

    public virtual async Task<int> ExecuteNonQueryAsync(DbCommand command)
    {
        using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
        {
            Database.PrepareCommand(command, openConnection.Connection);
            return await this.DoExecuteNonQueryAsync(command);
        }
    }

    public virtual int ExecuteNonQuery(DbCommand command, DbTransaction transaction)
    {
      Database.PrepareCommand(command, transaction);
      return this.DoExecuteNonQuery(command);
    }

    public virtual async Task<int> ExecuteNonQueryAsync(DbCommand command, DbTransaction transaction)
    {
        Database.PrepareCommand(command, transaction);
        return await this.DoExecuteNonQueryAsync(command);
    }

    public virtual int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteNonQuery(storedProcCommand);
    }

    public virtual async Task<int> ExecuteNonQueryAsync(string storedProcedureName, params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return await this.ExecuteNonQueryAsync(storedProcCommand);
    }

    public virtual int ExecuteNonQuery(
      DbTransaction transaction,
      string storedProcedureName,
      params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteNonQuery(storedProcCommand, transaction);
    }

    public virtual async Task<int> ExecuteNonQueryAsync(
        DbTransaction transaction,
        string storedProcedureName,
        params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return await this.ExecuteNonQueryAsync(storedProcCommand, transaction);
    }

    public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteNonQuery(commandByCommandType);
    }

    public virtual async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return await this.ExecuteNonQueryAsync(commandByCommandType);
    }

    public virtual int ExecuteNonQuery(
      DbTransaction transaction,
      CommandType commandType,
      string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteNonQuery(commandByCommandType, transaction);
    }

    public virtual async Task<int> ExecuteNonQueryAsync(
        DbTransaction transaction,
        CommandType commandType,
        string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return await this.ExecuteNonQueryAsync(commandByCommandType, transaction);
    }
    public virtual IDataReader ExecuteReader(DbCommand command)
    {
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        Database.PrepareCommand(command, openConnection.Connection);
        IDataReader innerReader = this.DoExecuteReader(command, CommandBehavior.Default);
        return this.CreateWrappedReader(openConnection, innerReader);
      }
    }

    public virtual async Task<IDataReader> ExecuteReaderAsync(DbCommand command)
    {
        using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
        {
            Database.PrepareCommand(command, openConnection.Connection);
            IDataReader innerReader = await this.DoExecuteReaderAsync(command, CommandBehavior.Default);
            return this.CreateWrappedReader(openConnection, innerReader);
        }
    }
    protected virtual IDataReader CreateWrappedReader(
      DatabaseConnectionWrapper connection,
      IDataReader innerReader)
    {
      return (IDataReader) new RefCountingDataReader(connection, innerReader);
    }

    public virtual IDataReader ExecuteReader(
      DbCommand command,
      DbTransaction transaction)
    {
      Database.PrepareCommand(command, transaction);
      return this.DoExecuteReader(command, CommandBehavior.Default);
    }

    public virtual async Task<IDataReader> ExecuteReaderAsync(
        DbCommand command,
        DbTransaction transaction)
    {
        Database.PrepareCommand(command, transaction);
        return await this.DoExecuteReaderAsync(command, CommandBehavior.Default);
    }

    public IDataReader ExecuteReader(
      string storedProcedureName,
      params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteReader(storedProcCommand);
    }

    public async Task<IDataReader> ExecuteReaderAsync(
            string storedProcedureName,
            params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return await this.ExecuteReaderAsync(storedProcCommand);
    }
    
    public IDataReader ExecuteReader(
      DbTransaction transaction,
      string storedProcedureName,
      params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteReader(storedProcCommand, transaction);
    }

    public async Task<IDataReader> ExecuteReaderAsync(
        DbTransaction transaction,
        string storedProcedureName,
        params object[] parameterValues)
    {
        using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return await this.ExecuteReaderAsync(storedProcCommand, transaction);
    }

    public IDataReader ExecuteReader(CommandType commandType, string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteReader(commandByCommandType);
    }

    public async Task<IDataReader> ExecuteReaderAsync(CommandType commandType, string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return await this.ExecuteReaderAsync(commandByCommandType);
    }

    public IDataReader ExecuteReader(
      DbTransaction transaction,
      CommandType commandType,
      string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteReader(commandByCommandType, transaction);
    }

    public async Task<IDataReader> ExecuteReaderAsync(
        DbTransaction transaction,
        CommandType commandType,
        string commandText)
    {
        using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return await this.ExecuteReaderAsync(commandByCommandType, transaction);
    }

    public virtual object ExecuteScalar(DbCommand command)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        Database.PrepareCommand(command, openConnection.Connection);
        return this.DoExecuteScalar((IDbCommand) command);
      }
    }

    public virtual object ExecuteScalar(DbCommand command, DbTransaction transaction)
    {
      Database.PrepareCommand(command, transaction);
      return this.DoExecuteScalar((IDbCommand) command);
    }

    public virtual object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteScalar(storedProcCommand);
    }

    public virtual object ExecuteScalar(
      DbTransaction transaction,
      string storedProcedureName,
      params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        return this.ExecuteScalar(storedProcCommand, transaction);
    }

    public virtual object ExecuteScalar(CommandType commandType, string commandText)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteScalar(commandByCommandType);
    }

    public virtual object ExecuteScalar(
      DbTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        return this.ExecuteScalar(commandByCommandType, transaction);
    }

    public DbDataAdapter GetDataAdapter()
    {
      return this.GetDataAdapter(UpdateBehavior.Standard);
    }

    protected DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior)
    {
      DbDataAdapter dataAdapter = this.dbProviderFactory.CreateDataAdapter();
      if (updateBehavior == UpdateBehavior.Continue)
        this.SetUpRowUpdatedEvent(dataAdapter);
      return dataAdapter;
    }

    internal DbConnection GetNewOpenConnection()
    {
      DbConnection dbConnection = (DbConnection) null;
      try
      {
        try
        {
          dbConnection = this.CreateConnection();
          dbConnection.Open();
        }
        catch (Exception ex)
        {
          this.instrumentationProvider.FireConnectionFailedEvent(this.ConnectionStringNoCredentials, ex);
          throw;
        }
        this.instrumentationProvider.FireConnectionOpenedEvent();
      }
      catch
      {
        dbConnection?.Close();
        throw;
      }
      return dbConnection;
    }

    protected DatabaseConnectionWrapper GetOpenConnection()
    {
      return TransactionScopeConnections.GetConnection(this) ?? this.GetWrappedConnection();
    }

    protected virtual DatabaseConnectionWrapper GetWrappedConnection()
    {
      return new DatabaseConnectionWrapper(this.GetNewOpenConnection());
    }

    public virtual object GetParameterValue(DbCommand command, string name)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      return command.Parameters[this.BuildParameterName(name)].Value;
    }

    public DbCommand GetSqlStringCommand(string query)
    {
      if (string.IsNullOrEmpty(query))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (query));
      return this.CreateCommandByCommandType(CommandType.Text, query);
    }

    public virtual DbCommand GetStoredProcCommand(string storedProcedureName)
    {
      if (string.IsNullOrEmpty(storedProcedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (storedProcedureName));
      return this.CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
    }

    public virtual DbCommand GetStoredProcCommand(
      string storedProcedureName,
      params object[] parameterValues)
    {
      if (string.IsNullOrEmpty(storedProcedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (storedProcedureName));
      DbCommand commandByCommandType = this.CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
      this.AssignParameters(commandByCommandType, parameterValues);
      return commandByCommandType;
    }

    public virtual void AssignParameters(DbCommand command, object[] parameterValues)
    {
      Database.parameterCache.SetParameters(command, this);
      if (!this.SameNumberOfParametersAndValues(command, parameterValues))
        throw new InvalidOperationException(Resources.ExceptionMessageParameterMatchFailure);
      this.AssignParameterValues(command, parameterValues);
    }

    public DbCommand GetStoredProcCommandWithSourceColumns(
      string storedProcedureName,
      params string[] sourceColumns)
    {
      if (string.IsNullOrEmpty(storedProcedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (storedProcedureName));
      if (sourceColumns == null)
        throw new ArgumentNullException(nameof (sourceColumns));
      DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName);
      using (DbConnection connection = this.CreateConnection())
      {
        storedProcCommand.Connection = connection;
        this.DiscoverParameters(storedProcCommand);
      }
      int index = 0;
      foreach (IDataParameter parameter in storedProcCommand.Parameters)
      {
        if (parameter.Direction == ParameterDirection.Input | parameter.Direction == ParameterDirection.InputOutput)
        {
          parameter.SourceColumn = sourceColumns[index];
          ++index;
        }
      }
      return storedProcCommand;
    }

    public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
    {
      this.LoadDataSet(command, dataSet, new string[1]
      {
        tableName
      });
    }

    public virtual void LoadDataSet(
      DbCommand command,
      DataSet dataSet,
      string tableName,
      DbTransaction transaction)
    {
      this.LoadDataSet(command, dataSet, new string[1]
      {
        tableName
      }, transaction);
    }

    public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
    {
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        Database.PrepareCommand(command, openConnection.Connection);
        this.DoLoadDataSet((IDbCommand) command, dataSet, tableNames);
      }
    }

    public virtual void LoadDataSet(
      DbCommand command,
      DataSet dataSet,
      string[] tableNames,
      DbTransaction transaction)
    {
      Database.PrepareCommand(command, transaction);
      this.DoLoadDataSet((IDbCommand) command, dataSet, tableNames);
    }

    public virtual void LoadDataSet(
      string storedProcedureName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        this.LoadDataSet(storedProcCommand, dataSet, tableNames);
    }

    public virtual void LoadDataSet(
      DbTransaction transaction,
      string storedProcedureName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.GetStoredProcCommand(storedProcedureName, parameterValues))
        this.LoadDataSet(storedProcCommand, dataSet, tableNames, transaction);
    }

    public virtual void LoadDataSet(
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        this.LoadDataSet(commandByCommandType, dataSet, tableNames);
    }

    public void LoadDataSet(
      DbTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      using (DbCommand commandByCommandType = this.CreateCommandByCommandType(commandType, commandText))
        this.LoadDataSet(commandByCommandType, dataSet, tableNames, transaction);
    }

    protected static void PrepareCommand(DbCommand command, DbConnection connection)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      command.Connection = connection;
    }

    protected static void PrepareCommand(DbCommand command, DbTransaction transaction)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      Database.PrepareCommand(command, transaction.Connection);
      command.Transaction = transaction;
    }

    private static void RollbackTransaction(IDbTransaction tran)
    {
      tran.Rollback();
    }

    protected virtual bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
    {
      return command.Parameters.Count == values.Length;
    }

    public virtual void SetParameterValue(DbCommand command, string parameterName, object value)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      command.Parameters[this.BuildParameterName(parameterName)].Value = value ?? (object) DBNull.Value;
    }

    protected virtual void SetUpRowUpdatedEvent(DbDataAdapter adapter)
    {
    }

    public int UpdateDataSet(
      DataSet dataSet,
      string tableName,
      DbCommand insertCommand,
      DbCommand updateCommand,
      DbCommand deleteCommand,
      UpdateBehavior updateBehavior,
      int? updateBatchSize)
    {
      using (DatabaseConnectionWrapper openConnection = this.GetOpenConnection())
      {
        if (updateBehavior == UpdateBehavior.Transactional && Transaction.Current == (Transaction) null)
        {
          DbTransaction transaction = Database.BeginTransaction(openConnection.Connection);
          try
          {
            int num = this.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, transaction, updateBatchSize);
            Database.CommitTransaction((IDbTransaction) transaction);
            return num;
          }
          catch
          {
            Database.RollbackTransaction((IDbTransaction) transaction);
            throw;
          }
        }
        else
        {
          if (insertCommand != null)
            Database.PrepareCommand(insertCommand, openConnection.Connection);
          if (updateCommand != null)
            Database.PrepareCommand(updateCommand, openConnection.Connection);
          if (deleteCommand != null)
            Database.PrepareCommand(deleteCommand, openConnection.Connection);
          return this.DoUpdateDataSet(updateBehavior, dataSet, tableName, (IDbCommand) insertCommand, (IDbCommand) updateCommand, (IDbCommand) deleteCommand, updateBatchSize);
        }
      }
    }

    public int UpdateDataSet(
      DataSet dataSet,
      string tableName,
      DbCommand insertCommand,
      DbCommand updateCommand,
      DbCommand deleteCommand,
      UpdateBehavior updateBehavior)
    {
      return this.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBehavior, new int?());
    }

    public int UpdateDataSet(
      DataSet dataSet,
      string tableName,
      DbCommand insertCommand,
      DbCommand updateCommand,
      DbCommand deleteCommand,
      DbTransaction transaction,
      int? updateBatchSize)
    {
      if (insertCommand != null)
        Database.PrepareCommand(insertCommand, transaction);
      if (updateCommand != null)
        Database.PrepareCommand(updateCommand, transaction);
      if (deleteCommand != null)
        Database.PrepareCommand(deleteCommand, transaction);
      return this.DoUpdateDataSet(UpdateBehavior.Transactional, dataSet, tableName, (IDbCommand) insertCommand, (IDbCommand) updateCommand, (IDbCommand) deleteCommand, updateBatchSize);
    }

    public int UpdateDataSet(
      DataSet dataSet,
      string tableName,
      DbCommand insertCommand,
      DbCommand updateCommand,
      DbCommand deleteCommand,
      DbTransaction transaction)
    {
      return this.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, transaction, new int?());
    }

    //public virtual bool SupportsAsync
    //{
    //  get
    //  {
    //    return false;
    //  }
    //}

    //private void AsyncNotSupported()
    //{
    //  throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AsyncOperationsNotSupported, (object) this.GetType().Name));
    //}

    protected virtual int UserParametersStartIndex()
    {
      return 0;
    }

    protected class OldConnectionWrapper : IDisposable
    {
      private readonly DbConnection connection;
      private readonly bool disposeConnection;

      public OldConnectionWrapper(DbConnection connection, bool disposeConnection)
      {
        this.connection = connection;
        this.disposeConnection = disposeConnection;
      }

      public DbConnection Connection
      {
        get
        {
          return this.connection;
        }
      }

      public void Dispose()
      {
        if (!this.disposeConnection)
          return;
        this.connection.Dispose();
      }
    }
  }
}
