// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.MapBuilder`1
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using Microsoft.Practices.Unity.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public static class MapBuilder<TResult> where TResult : new()
  {
    public static IRowMapper<TResult> BuildAllProperties()
    {
      return MapBuilder<TResult>.MapAllProperties().Build();
    }

    public static IMapBuilderContext<TResult> MapAllProperties()
    {
      IMapBuilderContext<TResult> mapBuilderContext = (IMapBuilderContext<TResult>) new MapBuilder<TResult>.MapBuilderContext();
      foreach (PropertyInfo property in ((IEnumerable<PropertyInfo>) typeof (TResult).GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (property => MapBuilder<TResult>.IsAutoMappableProperty(property))))
        mapBuilderContext = mapBuilderContext.MapByName(property);
      return mapBuilderContext;
    }

    public static IMapBuilderContext<TResult> MapNoProperties()
    {
      return (IMapBuilderContext<TResult>) new MapBuilder<TResult>.MapBuilderContext();
    }

    private static bool IsAutoMappableProperty(PropertyInfo property)
    {
      return property.CanWrite && property.GetIndexParameters().Length == 0 && !MapBuilder<TResult>.IsCollectionType(property.PropertyType);
    }

    private static bool IsCollectionType(Type type)
    {
      return type != typeof (string) && ((IEnumerable<Type>) type.GetInterfaces()).Where<Type>((Func<Type, bool>) (inf =>
      {
        if (inf == typeof (IEnumerable))
          return true;
        return inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof (IEnumerable<>);
      })).Count<Type>() != 0;
    }

    private class MapBuilderContext : IMapBuilderContextTest<TResult>, IMapBuilderContext<TResult>, IFluentInterface
    {
      private Dictionary<PropertyInfo, PropertyMapping> mappings;

      public MapBuilderContext()
      {
        this.mappings = new Dictionary<PropertyInfo, PropertyMapping>();
      }

      public IMapBuilderContextMap<TResult, TMember> Map<TMember>(
        Expression<Func<TResult, TMember>> propertySelector)
      {
        return (IMapBuilderContextMap<TResult, TMember>) new MapBuilder<TResult>.MapBuilderContext.MapBuilderContextMap<TMember>(this, MapBuilder<TResult>.MapBuilderContext.ExtractPropertyInfo<TMember>(propertySelector));
      }

      public IMapBuilderContextMap<TResult, object> Map(PropertyInfo property)
      {
        return (IMapBuilderContextMap<TResult, object>) new MapBuilder<TResult>.MapBuilderContext.MapBuilderContextMap<object>(this, MapBuilder<TResult>.MapBuilderContext.NormalizePropertyInfo(property));
      }

      public IMapBuilderContext<TResult> MapByName<TMember>(
        Expression<Func<TResult, TMember>> propertySelector)
      {
        return this.MapByName(MapBuilder<TResult>.MapBuilderContext.ExtractPropertyInfo<TMember>(propertySelector));
      }

      public IMapBuilderContext<TResult> MapByName(PropertyInfo property)
      {
        PropertyInfo property1 = MapBuilder<TResult>.MapBuilderContext.NormalizePropertyInfo(property);
        return this.Map(property1).ToColumn(property1.Name);
      }

      public IMapBuilderContext<TResult> DoNotMap<TMember>(
        Expression<Func<TResult, TMember>> propertySelector)
      {
        return this.DoNotMap(MapBuilder<TResult>.MapBuilderContext.ExtractPropertyInfo<TMember>(propertySelector));
      }

      public IMapBuilderContext<TResult> DoNotMap(PropertyInfo property)
      {
        this.mappings.Remove(MapBuilder<TResult>.MapBuilderContext.NormalizePropertyInfo(property));
        return (IMapBuilderContext<TResult>) this;
      }

      public IRowMapper<TResult> Build()
      {
        return (IRowMapper<TResult>) new ReflectionRowMapper<TResult>((IDictionary<PropertyInfo, PropertyMapping>) this.mappings);
      }

      public IEnumerable<PropertyMapping> GetPropertyMappings()
      {
        return (IEnumerable<PropertyMapping>) this.mappings.Values;
      }

      private static PropertyInfo ExtractPropertyInfo<TMember>(
        Expression<Func<TResult, TMember>> propertySelector)
      {
        if (!(propertySelector.Body is MemberExpression body))
          throw new ArgumentException(Resources.ExceptionArgumentMustBePropertyExpression, nameof (propertySelector));
        if (!(body.Member is PropertyInfo member))
          throw new ArgumentException(Resources.ExceptionArgumentMustBePropertyExpression, nameof (propertySelector));
        return MapBuilder<TResult>.MapBuilderContext.NormalizePropertyInfo(member);
      }

      private static PropertyInfo NormalizePropertyInfo(PropertyInfo property)
      {
        if (property == null)
          throw new ArgumentNullException(nameof (property));
        return typeof (TResult).GetProperty(property.Name);
      }

      Type IFluentInterface.GetType()
      {
        return this.GetType();
      }

      private class MapBuilderContextMap<TMember> : IMapBuilderContextMap<TResult, TMember>, IFluentInterface
      {
        private PropertyInfo property;
        private MapBuilder<TResult>.MapBuilderContext builderContext;

        public MapBuilderContextMap(
          MapBuilder<TResult>.MapBuilderContext builderContext,
          PropertyInfo property)
        {
          this.property = property;
          this.builderContext = builderContext;
        }

        public IMapBuilderContext<TResult> ToColumn(string columnName)
        {
          this.builderContext.mappings[this.property] = (PropertyMapping) new ColumnNameMapping(this.property, columnName);
          return (IMapBuilderContext<TResult>) this.builderContext;
        }

        public IMapBuilderContext<TResult> WithFunc(Func<IDataRecord, TMember> f)
        {
          Guard.ArgumentNotNull((object) f, nameof (f));
          this.builderContext.mappings[this.property] = (PropertyMapping) new FuncMapping(this.property, (Func<IDataRecord, object>) (row => (object) f(row)));
          return (IMapBuilderContext<TResult>) this.builderContext;
        }

        Type IFluentInterface.GetType()
        {
          return this.GetType();
        }
      }
    }
  }
}
