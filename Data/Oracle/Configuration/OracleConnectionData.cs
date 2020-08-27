// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OracleConnectionData
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration
{
  [NameProperty("Name", NamePropertyDisplayFormat = "Oracle Packages for {0}")]
  [ResourceDescription(typeof (DesignResources), "OracleConnectionDataDescription")]
  [ResourceDisplayName(typeof (DesignResources), "OracleConnectionDataDisplayName")]
  public class OracleConnectionData : NamedConfigurationElement
  {
    private const string packagesProperty = "packages";

    [ResourceDescription(typeof (DesignResources), "OracleConnectionDataNameDescription")]
    [Reference(typeof (ConnectionStringSettingsCollection), typeof (ConnectionStringSettings))]
    [ResourceDisplayName(typeof (DesignResources), "OracleConnectionDataNameDisplayName")]
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

    [ResourceDisplayName(typeof (DesignResources), "OracleConnectionDataPackagesDisplayName")]
    [EnvironmentalOverrides(false)]
    [ConfigurationCollection(typeof (OraclePackageData))]
    [ConfigurationProperty("packages", IsRequired = true)]
    [ResourceDescription(typeof (DesignResources), "OracleConnectionDataPackagesDescription")]
    [Editor("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.ElementCollectionEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime", "System.Windows.FrameworkElement, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    [DesignTimeReadOnly(false)]
    public NamedElementCollection<OraclePackageData> Packages
    {
      get
      {
        return (NamedElementCollection<OraclePackageData>) this["packages"];
      }
    }
  }
}
