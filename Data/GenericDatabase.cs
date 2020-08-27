// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.GenericDatabase
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Data.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  [ConfigurationElementType(typeof (GenericDatabaseData))]
  public class GenericDatabase : Database
  {
    public GenericDatabase(string connectionString, DbProviderFactory dbProviderFactory)
      : base(connectionString, dbProviderFactory)
    {
    }

    public GenericDatabase(
      string connectionString,
      DbProviderFactory dbProviderFactory,
      IDataInstrumentationProvider instrumentationProvider)
      : base(connectionString, dbProviderFactory, instrumentationProvider)
    {
    }

    protected override void DeriveParameters(DbCommand discoveryCommand)
    {
      throw new NotSupportedException(Resources.ExceptionParameterDiscoveryNotSupportedOnGenericDatabase);
    }
  }
}
