// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.PropertyMapping
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public abstract class PropertyMapping
  {
    protected PropertyMapping(PropertyInfo property)
    {
      this.Property = property;
    }

    public PropertyInfo Property { get; private set; }

    public abstract object GetPropertyValue(IDataRecord row);

    public void Map(object instance, IDataRecord row)
    {
      object propertyValue = this.GetPropertyValue(row);
      this.SetValue(instance, propertyValue);
    }

    protected void SetValue(object instance, object value)
    {
      this.Property.SetValue(instance, value, new object[0]);
    }

    protected static object ConvertValue(object value, Type conversionType)
    {
      return PropertyMapping.IsNullableType(conversionType) ? PropertyMapping.ConvertNullableValue(value, conversionType) : PropertyMapping.ConvertNonNullableValue(value, conversionType);
    }

    private static bool IsNullableType(Type t)
    {
      return t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    protected static object ConvertNullableValue(object value, Type conversionType)
    {
      return value != DBNull.Value ? new NullableConverter(conversionType).ConvertFrom(value) : (object) null;
    }

    protected static object ConvertNonNullableValue(object value, Type conversionType)
    {
      object obj = (object) null;
      if (value != DBNull.Value)
        obj = Convert.ChangeType(value, conversionType);
      else if (conversionType.IsValueType)
        obj = Activator.CreateInstance(conversionType);
      return obj;
    }
  }
}
