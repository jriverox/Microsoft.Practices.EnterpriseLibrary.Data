﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Sql.Configuration.SqlDatabaseData
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Sql.Configuration
{
  public class SqlDatabaseData : DatabaseData
  {
    public SqlDatabaseData(
      ConnectionStringSettings connectionStringSettings,
      IConfigurationSource configurationSource)
      : base(connectionStringSettings, configurationSource)
    {
    }

    public override IEnumerable<TypeRegistration> GetRegistrations()
    {
      TypeRegistration<Database> typeRegistration = new TypeRegistration<Database>((Expression<Func<Database>>) (() => new SqlDatabase(this.ConnectionString, Container.Resolved<IDataInstrumentationProvider>(this.Name))));
      typeRegistration.Name = this.Name;
      typeRegistration.Lifetime = TypeRegistrationLifetime.Transient;
      yield return (TypeRegistration) typeRegistration;
    }
  }
}
