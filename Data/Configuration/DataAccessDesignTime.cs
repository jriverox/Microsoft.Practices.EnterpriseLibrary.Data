// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DataAccessDesignTime
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration
{
  public static class DataAccessDesignTime
  {
    public const string ConnectionStringSettingsSectionName = "connectionStrings";

    internal static class ConverterTypeNames
    {
      public const string SystemDataConverter = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Converters.SystemDataProviderConverter, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
    }

    internal static class ViewModelTypeNames
    {
      public const string ConnectionStringPropertyViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ConnectionStringPropertyViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
      public const string DataSectionViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.DataSectionViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
      public const string OraclePackageDataViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.OraclePackageDataViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
    }

    internal static class CommandTypeNames
    {
      public const string AddDataAccessBlockCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.AddDatabaseBlockCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
    }

    public static class MetadataTypes
    {
      [RegisterAsMetadataType(typeof (DbProviderMapping))]
      public abstract class DbProviderMappingMetadata
      {
        [TypeConverter("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Converters.SystemDataProviderConverter, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime")]
        public string Name { get; set; }
      }

      [RegisterAsMetadataType(typeof (ConnectionStringsSection))]
      [ResourceDisplayName(typeof (DesignResources), "ConnectionStringsSectionMetadataDisplayName")]
      [ViewModel("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.DataSectionViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime")]
      [ResourceDescription(typeof (DesignResources), "ConnectionStringsSectionMetadataDescription")]
      public abstract class ConnectionStringsSectionMetadata
      {
      }

      [ResourceDisplayName(typeof (DesignResources), "ConnectionStringSettingsCollectionMetadataDisplayName")]
      [RegisterAsMetadataType(typeof (ConnectionStringSettingsCollection))]
      [ResourceDescription(typeof (DesignResources), "ConnectionStringSettingsCollectionMetadataDescription")]
      public abstract class ConnectionStringSettingsCollectionMetadata
      {
      }

      [RegisterAsMetadataType(typeof (ConnectionStringSettings))]
      [ResourceDisplayName(typeof (DesignResources), "ConnectionStringSettingsMetadataDisplayName")]
      [ResourceDescription(typeof (DesignResources), "ConnectionStringSettingsMetadataDescription")]
      [NameProperty("Name")]
      public abstract class ConnectionStringSettingsMetadata
      {
        [ResourceDisplayName(typeof (DesignResources), "ConnectionStringSettingsMetadataNameDisplayName")]
        [EnvironmentalOverrides(false)]
        [ResourceCategory(typeof (DesignResources), "CategoryName")]
        [ResourceDescription(typeof (DesignResources), "ConnectionStringSettingsMetadataNameDescription")]
        public string Name { get; set; }

        [ResourceDisplayName(typeof (DesignResources), "ConnectionStringSettingsMetadataConnectionStringDisplayName")]
        [EditorWithReadOnlyText(true)]
        [ViewModel("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ConnectionStringPropertyViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime")]
        [ResourceDescription(typeof (DesignResources), "ConnectionStringSettingsMetadataConnectionStringDescription")]
        [Editor("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.PopupTextEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string ConnectionString { get; set; }

        [TypeConverter("Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Converters.SystemDataProviderConverter, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime")]
        [ResourceDisplayName(typeof (DesignResources), "ConnectionStringSettingsMetadataProviderNameDisplayName")]
        [ResourceDescription(typeof (DesignResources), "ConnectionStringSettingsMetadataProviderNameDescription")]
        public string ProviderName { get; set; }
      }
    }
  }
}
