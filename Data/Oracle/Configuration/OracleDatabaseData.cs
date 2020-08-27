// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OracleDatabaseData
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
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration
{
  public class OracleDatabaseData : DatabaseData
  {
    public OracleDatabaseData(
      ConnectionStringSettings connectionStringSettings,
      IConfigurationSource configurationSource)
      : base(connectionStringSettings, configurationSource)
    {
      OracleConnectionSettings section = (OracleConnectionSettings) configurationSource.GetSection("oracleConnectionSettings");
      if (section == null)
        return;
      this.ConnectionData = section.OracleConnectionsData.Get(connectionStringSettings.Name);
    }

    public IEnumerable<OraclePackageData> PackageMappings
    {
      get
      {
        return this.ConnectionData == null ? (IEnumerable<OraclePackageData>) new OraclePackageData[0] : (IEnumerable<OraclePackageData>) this.ConnectionData.Packages;
      }
    }

    private OracleConnectionData ConnectionData { get; set; }

    public override IEnumerable<TypeRegistration> GetRegistrations()
    {
      TypeRegistration<Database> typeRegistration = new TypeRegistration<Database>((Expression<Func<Database>>) (() => new OracleDatabase(this.ConnectionString, this.PackageMappings.Select<OraclePackageData, IOraclePackage>((Func<OraclePackageData, IOraclePackage>) (opd => (IOraclePackage) opd)), Container.Resolved<IDataInstrumentationProvider>(this.Name))));
      typeRegistration.Name = this.Name;
      typeRegistration.Lifetime = TypeRegistrationLifetime.Transient;
      yield return (TypeRegistration) typeRegistration;
    }
  }
}
