// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OraclePackageData
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration
{
  [ResourceDescription(typeof (DesignResources), "OraclePackageDataDescription")]
  [ResourceDisplayName(typeof (DesignResources), "OraclePackageDataDisplayName")]
  public class OraclePackageData : NamedConfigurationElement, IOraclePackage
  {
    private const string prefixProperty = "prefix";

    public OraclePackageData()
    {
      this.Prefix = string.Empty;
    }

    public OraclePackageData(string name, string prefix)
      : base(name)
    {
      this.Prefix = prefix;
    }

    [ConfigurationProperty("prefix", IsRequired = true)]
    [ViewModel("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.CollectionEditorContainedElementProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime")]
    [ResourceDisplayName(typeof (DesignResources), "OraclePackageDataPrefixDisplayName")]
    [ResourceDescription(typeof (DesignResources), "OraclePackageDataPrefixDescription")]
    public string Prefix
    {
      get
      {
        return (string) this["prefix"];
      }
      set
      {
        this["prefix"] = (object) value;
      }
    }

    [ViewModel("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.CollectionEditorContainedElementProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime")]
    public override string Name
    {
      get
      {
        return base.Name;
      }
      set
      {
        base.Name = value;
      }
    }
  }
}
