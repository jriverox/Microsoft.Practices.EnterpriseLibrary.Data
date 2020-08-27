// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.FuncMapping
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.Unity.Utility;
using System.Data;
using System.Reflection;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class FuncMapping : PropertyMapping
  {
    public FuncMapping(PropertyInfo property, System.Func<IDataRecord, object> func)
      : base(property)
    {
      Guard.ArgumentNotNull((object) func, nameof (func));
      this.Func = func;
    }

    public System.Func<IDataRecord, object> Func { get; private set; }

    public override object GetPropertyValue(IDataRecord row)
    {
      return this.Func(row);
    }
  }
}
