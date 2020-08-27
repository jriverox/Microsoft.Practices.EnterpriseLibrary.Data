// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.SprocAccessor`1
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class SprocAccessor<TResult> : CommandAccessor<TResult>
  {
    private readonly IParameterMapper parameterMapper;
    private readonly string procedureName;

    public SprocAccessor(Database database, string procedureName, IRowMapper<TResult> rowMapper)
      : this(database, procedureName, (IParameterMapper) new SprocAccessor<TResult>.DefaultParameterMapper(database), rowMapper)
    {
    }

    public SprocAccessor(
      Database database,
      string procedureName,
      IResultSetMapper<TResult> resultSetMapper)
      : this(database, procedureName, (IParameterMapper) new SprocAccessor<TResult>.DefaultParameterMapper(database), resultSetMapper)
    {
    }

    public SprocAccessor(
      Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      IRowMapper<TResult> rowMapper)
      : base(database, rowMapper)
    {
      if (string.IsNullOrEmpty(procedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      if (parameterMapper == null)
        throw new ArgumentNullException(nameof (parameterMapper));
      this.procedureName = procedureName;
      this.parameterMapper = parameterMapper;
    }

    public SprocAccessor(
      Database database,
      string procedureName,
      IParameterMapper parameterMapper,
      IResultSetMapper<TResult> resultSetMapper)
      : base(database, resultSetMapper)
    {
      if (string.IsNullOrEmpty(procedureName))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString);
      if (parameterMapper == null)
        throw new ArgumentNullException(nameof (parameterMapper));
      this.procedureName = procedureName;
      this.parameterMapper = parameterMapper;
    }

    public override IEnumerable<TResult> Execute(params object[] parameterValues)
    {
      using (DbCommand storedProcCommand = this.Database.GetStoredProcCommand(this.procedureName))
      {
        this.parameterMapper.AssignParameters(storedProcCommand, parameterValues);
        return this.Execute(storedProcCommand);
      }
    }

    private class DefaultParameterMapper : IParameterMapper
    {
      private readonly Database database;

      public DefaultParameterMapper(Database database)
      {
        this.database = database;
      }

      public void AssignParameters(DbCommand command, object[] parameterValues)
      {
        if (parameterValues.Length <= 0)
          return;
        this.GuardParameterDiscoverySupported();
        this.database.AssignParameters(command, parameterValues);
      }

      private void GuardParameterDiscoverySupported()
      {
        if (!this.database.SupportsParemeterDiscovery)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionParameterDiscoveryNotSupported, (object) this.database.GetType().FullName));
      }
    }
  }
}
