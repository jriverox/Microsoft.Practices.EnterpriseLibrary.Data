// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseData
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration
{
  public abstract class DatabaseData
  {
    protected DatabaseData(
      ConnectionStringSettings connectionStringSettings,
      IConfigurationSource configurationSource)
    {
      this.ConnectionStringSettings = connectionStringSettings;
      this.ConfigurationSource = configurationSource;
    }

    protected ConnectionStringSettings ConnectionStringSettings { get; private set; }

    protected IConfigurationSource ConfigurationSource { get; private set; }

    public string Name
    {
      get
      {
        return this.ConnectionStringSettings.Name;
      }
    }

    public string ConnectionString
    {
      get
      {
        return this.ConnectionStringSettings.ConnectionString;
      }
    }

    public abstract IEnumerable<TypeRegistration> GetRegistrations();
  }
}
