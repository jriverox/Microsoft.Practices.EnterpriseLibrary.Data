// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.ColumnNameMapping
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class ColumnNameMapping : PropertyMapping
  {
    public ColumnNameMapping(PropertyInfo property, string columnName)
      : base(property)
    {
      this.ColumnName = columnName;
    }

    public string ColumnName { get; private set; }

    public override object GetPropertyValue(IDataRecord row)
    {
      if (row == null)
        throw new ArgumentNullException(nameof (row));
      object obj;
      try
      {
        obj = row[this.ColumnName];
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionColumnNotFoundWhileMapping, (object) this.ColumnName), (Exception) ex);
      }
      try
      {
        return PropertyMapping.ConvertValue(obj, this.Property.PropertyType);
      }
      catch (InvalidCastException ex)
      {
        throw new InvalidCastException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionConvertionFailedWhenMappingPropertyToColumn, (object) this.ColumnName, (object) this.Property.Name, (object) this.Property.PropertyType), (Exception) ex);
      }
      catch (FormatException ex)
      {
        throw new InvalidCastException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionConvertionFailedWhenMappingPropertyToColumn, (object) this.ColumnName, (object) this.Property.Name, (object) this.Property.PropertyType), (Exception) ex);
      }
    }
  }
}
