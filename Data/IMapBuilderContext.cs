// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.IMapBuilderContext`1
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public interface IMapBuilderContext<TResult> : IFluentInterface
  {
    IMapBuilderContext<TResult> MapByName(PropertyInfo property);

    IMapBuilderContext<TResult> MapByName<TMember>(
      Expression<Func<TResult, TMember>> propertySelector);

    IMapBuilderContext<TResult> DoNotMap(PropertyInfo property);

    IMapBuilderContext<TResult> DoNotMap<TMember>(
      Expression<Func<TResult, TMember>> propertySelector);

    IMapBuilderContextMap<TResult, TMember> Map<TMember>(
      Expression<Func<TResult, TMember>> propertySelector);

    IMapBuilderContextMap<TResult, object> Map(PropertyInfo property);

    IRowMapper<TResult> Build();
  }
}
