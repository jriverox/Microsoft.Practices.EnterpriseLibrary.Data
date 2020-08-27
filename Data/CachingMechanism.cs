// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.CachingMechanism
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.Collections;
using System.Data;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  internal class CachingMechanism
  {
    private Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

    public static IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
    {
      IDataParameter[] dataParameterArray = new IDataParameter[originalParameters.Length];
      int index = 0;
      for (int length = originalParameters.Length; index < length; ++index)
        dataParameterArray[index] = (IDataParameter) ((ICloneable) originalParameters[index]).Clone();
      return dataParameterArray;
    }

    public void Clear()
    {
      this.paramCache.Clear();
    }

    public void AddParameterSetToCache(
      string connectionString,
      IDbCommand command,
      IDataParameter[] parameters)
    {
      string commandText = command.CommandText;
      this.paramCache[(object) CachingMechanism.CreateHashKey(connectionString, commandText)] = (object) parameters;
    }

    public IDataParameter[] GetCachedParameterSet(
      string connectionString,
      IDbCommand command)
    {
      string commandText = command.CommandText;
      return CachingMechanism.CloneParameters((IDataParameter[]) this.paramCache[(object) CachingMechanism.CreateHashKey(connectionString, commandText)]);
    }

    public bool IsParameterSetCached(string connectionString, IDbCommand command)
    {
      return this.paramCache[(object) CachingMechanism.CreateHashKey(connectionString, command.CommandText)] != null;
    }

    private static string CreateHashKey(string connectionString, string storedProcedure)
    {
      return connectionString + ":" + storedProcedure;
    }
  }
}
