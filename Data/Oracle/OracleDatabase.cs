// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.OracleDatabase
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Globalization;
using System.Security.Permissions;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle
{
  [ConfigurationElementType(typeof (OracleDatabaseData))]
  [OraclePermission(SecurityAction.Demand)]
  public class OracleDatabase : Database
  {
    private static readonly IEnumerable<IOraclePackage> emptyPackages = (IEnumerable<IOraclePackage>) new List<IOraclePackage>(0);
    private readonly IDictionary<string, ParameterTypeRegistry> registeredParameterTypes = (IDictionary<string, ParameterTypeRegistry>) new Dictionary<string, ParameterTypeRegistry>();
    private const string RefCursorName = "cur_OUT";
    private readonly IEnumerable<IOraclePackage> packages;

    public OracleDatabase(string connectionString)
      : this(connectionString, OracleDatabase.emptyPackages)
    {
    }

    public OracleDatabase(string connectionString, IEnumerable<IOraclePackage> packages)
      : this(connectionString, packages, (IDataInstrumentationProvider) new NullDataInstrumentationProvider())
    {
    }

    public OracleDatabase(
      string connectionString,
      IEnumerable<IOraclePackage> packages,
      IDataInstrumentationProvider instrumentationProvider)
      : base(connectionString, (DbProviderFactory) OracleClientFactory.Instance, instrumentationProvider)
    {
      if (packages == null)
        throw new ArgumentNullException(nameof (packages));
      this.packages = packages;
    }

    public override void AddParameter(
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
      if (DbType.Guid.Equals((object) dbType))
      {
        object byteArray = OracleDatabase.ConvertGuidToByteArray(value);
        this.AddParameter((OracleCommand) command, name, OracleType.Raw, 16, direction, nullable, precision, scale, sourceColumn, sourceVersion, byteArray);
        this.RegisterParameterType(command, name, dbType);
      }
      else
        base.AddParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
    }

    public void AddParameter(
      OracleCommand command,
      string name,
      OracleType oracleType,
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
      OracleParameter parameter = this.CreateParameter(name, DbType.AnsiString, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value) as OracleParameter;
      parameter.OracleType = oracleType;
      command.Parameters.Add(parameter);
    }

    public override IDataReader ExecuteReader(DbCommand command)
    {
      this.PrepareCWRefCursor(command);
      return base.ExecuteReader(command);
    }

    protected override IDataReader CreateWrappedReader(
      DatabaseConnectionWrapper connection,
      IDataReader innerReader)
    {
      return (IDataReader) new RefCountingOracleDataReaderWrapper(connection, (OracleDataReader) innerReader);
    }

    public override IDataReader ExecuteReader(
      DbCommand command,
      DbTransaction transaction)
    {
      this.PrepareCWRefCursor(command);
      return (IDataReader) new OracleDataReaderWrapper((OracleDataReader) base.ExecuteReader(command, transaction));
    }

    public override DataSet ExecuteDataSet(DbCommand command)
    {
      this.PrepareCWRefCursor(command);
      return base.ExecuteDataSet(command);
    }

    public override DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
    {
      this.PrepareCWRefCursor(command);
      return base.ExecuteDataSet(command, transaction);
    }

    public override void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
    {
      this.PrepareCWRefCursor(command);
      base.LoadDataSet(command, dataSet, tableNames);
    }

    public override void LoadDataSet(
      DbCommand command,
      DataSet dataSet,
      string[] tableNames,
      DbTransaction transaction)
    {
      this.PrepareCWRefCursor(command);
      base.LoadDataSet(command, dataSet, tableNames, transaction);
    }

    public override object GetParameterValue(DbCommand command, string parameterName)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      object obj = base.GetParameterValue(command, parameterName);
      ParameterTypeRegistry parameterTypeRegistry = this.GetParameterTypeRegistry(command.CommandText);
      if (parameterTypeRegistry != null && parameterTypeRegistry.HasRegisteredParameterType(parameterName))
      {
        DbType registeredParameterType = parameterTypeRegistry.GetRegisteredParameterType(parameterName);
        if (DbType.Guid == registeredParameterType)
          obj = OracleDatabase.ConvertByteArrayToGuid(obj);
        else if (DbType.Boolean == registeredParameterType)
          obj = (object) Convert.ToBoolean(obj, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      return obj;
    }

    public override void SetParameterValue(DbCommand command, string parameterName, object value)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      object obj = value;
      ParameterTypeRegistry parameterTypeRegistry = this.GetParameterTypeRegistry(command.CommandText);
      if (parameterTypeRegistry != null && parameterTypeRegistry.HasRegisteredParameterType(parameterName) && DbType.Guid == parameterTypeRegistry.GetRegisteredParameterType(parameterName))
        obj = OracleDatabase.ConvertGuidToByteArray(value);
      base.SetParameterValue(command, parameterName, obj);
    }

    private void PrepareCWRefCursor(DbCommand command)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (CommandType.StoredProcedure != command.CommandType || !OracleDatabase.QueryProcedureNeedsCursorParameter(command))
        return;
      this.AddParameter(command as OracleCommand, "cur_OUT", OracleType.Cursor, 0, ParameterDirection.Output, true, (byte) 0, (byte) 0, string.Empty, DataRowVersion.Default, Convert.DBNull);
    }

    private ParameterTypeRegistry GetParameterTypeRegistry(string commandText)
    {
      ParameterTypeRegistry parameterTypeRegistry;
      this.registeredParameterTypes.TryGetValue(commandText, out parameterTypeRegistry);
      return parameterTypeRegistry;
    }

    private void RegisterParameterType(DbCommand command, string parameterName, DbType dbType)
    {
      ParameterTypeRegistry parameterTypeRegistry = this.GetParameterTypeRegistry(command.CommandText);
      if (parameterTypeRegistry == null)
      {
        parameterTypeRegistry = new ParameterTypeRegistry(command.CommandText);
        this.registeredParameterTypes.Add(command.CommandText, parameterTypeRegistry);
      }
      parameterTypeRegistry.RegisterParameterType(parameterName, dbType);
    }

    private static object ConvertGuidToByteArray(object value)
    {
      switch (value)
      {
        case DBNull _:
        case null:
          return Convert.DBNull;
        default:
          return (object) ((Guid) value).ToByteArray();
      }
    }

    private static object ConvertByteArrayToGuid(object value)
    {
      byte[] b = (byte[]) value;
      return b.Length == 0 ? (object) DBNull.Value : (object) new Guid(b);
    }

    private static bool QueryProcedureNeedsCursorParameter(DbCommand command)
    {
      foreach (OracleParameter parameter in command.Parameters)
      {
        if (parameter.OracleType == OracleType.Cursor)
          return false;
      }
      return true;
    }

    private void OnOracleRowUpdated(object sender, OracleRowUpdatedEventArgs args)
    {
      if (args.RecordsAffected != 0 || args.Errors == null)
        return;
      args.Row.RowError = Resources.ExceptionMessageUpdateDataSetRowFailure;
      args.Status = UpdateStatus.SkipCurrentRow;
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
      OracleCommandBuilder.DeriveParameters((OracleCommand) discoveryCommand);
    }

    public override DbCommand GetStoredProcCommand(
      string storedProcedureName,
      params object[] parameterValues)
    {
      return base.GetStoredProcCommand(this.TranslatePackageSchema(storedProcedureName), parameterValues);
    }

    public override void AssignParameters(DbCommand command, object[] parameterValues)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      this.TranslatePackageSchema(command.CommandText);
      base.AssignParameters(command, parameterValues);
    }

    public override DbCommand GetStoredProcCommand(string storedProcedureName)
    {
      return base.GetStoredProcCommand(this.TranslatePackageSchema(storedProcedureName));
    }

    private string TranslatePackageSchema(string storedProcedureName)
    {
      string str1 = string.Empty;
      string str2 = storedProcedureName;
      if (this.packages != null && !string.IsNullOrEmpty(storedProcedureName))
      {
        foreach (IOraclePackage package in this.packages)
        {
          if (package.Prefix == "*" || storedProcedureName.StartsWith(package.Prefix))
          {
            str1 = package.Name;
            break;
          }
        }
      }
      if (str1.Length != 0)
        str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) str1, (object) storedProcedureName);
      return str2;
    }

    protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter)
    {
      ((OracleDataAdapter) adapter).RowUpdated += new OracleRowUpdatedEventHandler(this.OnOracleRowUpdated);
    }
  }
}
