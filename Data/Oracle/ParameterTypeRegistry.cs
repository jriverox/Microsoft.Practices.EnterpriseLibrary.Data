// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.ParameterTypeRegistry
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle
{
  internal sealed class ParameterTypeRegistry
  {
    private string commandText;
    private IDictionary<string, DbType> parameterTypes;

    internal ParameterTypeRegistry(string commandText)
    {
      this.commandText = commandText;
      this.parameterTypes = (IDictionary<string, DbType>) new Dictionary<string, DbType>();
    }

    internal void RegisterParameterType(string parameterName, DbType parameterType)
    {
      this.parameterTypes[parameterName] = parameterType;
    }

    internal bool HasRegisteredParameterType(string parameterName)
    {
      return this.parameterTypes.ContainsKey(parameterName);
    }

    internal DbType GetRegisteredParameterType(string parameterName)
    {
      return this.parameterTypes[parameterName];
    }
  }
}
