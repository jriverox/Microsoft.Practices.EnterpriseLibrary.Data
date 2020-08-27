// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DbProviderMapping
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using System;
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration
{
  [ResourceDisplayName(typeof (DesignResources), "DbProviderMappingDisplayName")]
  [ResourceDescription(typeof (DesignResources), "DbProviderMappingDescription")]
  public class DbProviderMapping : NamedConfigurationElement
  {
    private static AssemblyQualifiedTypeNameConverter typeConverter = new AssemblyQualifiedTypeNameConverter();
    public const string DefaultSqlProviderName = "System.Data.SqlClient";
    public const string DefaultOracleProviderName = "System.Data.OracleClient";
    internal const string DefaultGenericProviderName = "generic";
    private const string databaseTypeProperty = "databaseType";

    public DbProviderMapping()
    {
    }

    public DbProviderMapping(string dbProviderName, Type databaseType)
      : this(dbProviderName, (string) DbProviderMapping.typeConverter.ConvertTo((object) databaseType, typeof (string)))
    {
    }

    public DbProviderMapping(string dbProviderName, string databaseTypeName)
      : base(dbProviderName)
    {
      this.DatabaseTypeName = databaseTypeName;
    }

    public Type DatabaseType
    {
      get
      {
        return (Type) DbProviderMapping.typeConverter.ConvertFrom((object) this.DatabaseTypeName);
      }
      set
      {
        this.DatabaseTypeName = DbProviderMapping.typeConverter.ConvertToString((object) value);
      }
    }

    [ResourceDisplayName(typeof (DesignResources), "DbProviderMappingDatabaseTypeNameDisplayName")]
    [BaseType(typeof (Database))]
    [ConfigurationProperty("databaseType")]
    [ResourceDescription(typeof (DesignResources), "DbProviderMappingDatabaseTypeNameDescription")]
    [Editor("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.TypeSelectionEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public string DatabaseTypeName
    {
      get
      {
        return (string) this["databaseType"];
      }
      set
      {
        this["databaseType"] = (object) value;
      }
    }

    public string DbProviderName
    {
      get
      {
        return this.Name;
      }
    }

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
