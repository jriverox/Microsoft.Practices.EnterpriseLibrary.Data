// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.ReflectionRowMapper`1
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using Microsoft.Practices.Unity.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class ReflectionRowMapper<TResult> : IRowMapper<TResult> where TResult : new()
  {
    private static readonly MethodInfo ConvertValue = StaticReflection.GetMethodInfo<PropertyMapping>((Expression<Action<PropertyMapping>>) (pm => pm.GetPropertyValue(default (IDataRecord))));
    private static readonly NewExpression CreationExpression = Expression.New(typeof (TResult));
    private readonly Func<IDataRecord, TResult> mapping;

    public ReflectionRowMapper(
      IDictionary<PropertyInfo, PropertyMapping> propertyMappings)
    {
      if (propertyMappings == null)
        throw new ArgumentNullException(nameof (propertyMappings));
      try
      {
        ParameterExpression parameter = Expression.Parameter(typeof (IDataRecord), "reader");
        IEnumerable<MemberBinding> bindings = propertyMappings.Select<KeyValuePair<PropertyInfo, PropertyMapping>, MemberBinding>((Func<KeyValuePair<PropertyInfo, PropertyMapping>, MemberBinding>) (kvp => (MemberBinding) Expression.Bind((MemberInfo) kvp.Key, (Expression) Expression.Convert((Expression) Expression.Call((Expression) Expression.Constant((object) kvp.Value), ReflectionRowMapper<TResult>.ConvertValue, (Expression) parameter), kvp.Key.PropertyType))));
        this.mapping = Expression.Lambda<Func<IDataRecord, TResult>>((Expression) Expression.MemberInit(ReflectionRowMapper<TResult>.CreationExpression, bindings), parameter).Compile();
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExceptionCannotCreateRowMapping, (object) typeof (TResult).Name), ex);
      }
    }

    public TResult MapRow(IDataRecord row)
    {
      return this.mapping(row);
    }
  }
}
