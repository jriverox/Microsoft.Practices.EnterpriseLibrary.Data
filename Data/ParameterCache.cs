// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.ParameterCache
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.Data;
using System.Data.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class ParameterCache
  {
    private CachingMechanism cache = new CachingMechanism();

    public void SetParameters(DbCommand command, Database database)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      if (this.AlreadyCached((IDbCommand) command, database))
      {
        this.AddParametersFromCache(command, database);
      }
      else
      {
        database.DiscoverParameters(command);
        IDataParameter[] parameterCopy = ParameterCache.CreateParameterCopy(command);
        this.cache.AddParameterSetToCache(database.ConnectionString, (IDbCommand) command, parameterCopy);
      }
    }

    protected internal void Clear()
    {
      this.cache.Clear();
    }

    protected virtual void AddParametersFromCache(DbCommand command, Database database)
    {
      foreach (IDataParameter cachedParameter in this.cache.GetCachedParameterSet(database.ConnectionString, (IDbCommand) command))
        command.Parameters.Add((object) cachedParameter);
    }

    private bool AlreadyCached(IDbCommand command, Database database)
    {
      return this.cache.IsParameterSetCached(database.ConnectionString, command);
    }

    private static IDataParameter[] CreateParameterCopy(DbCommand command)
    {
      IDataParameterCollection parameters = (IDataParameterCollection) command.Parameters;
      IDataParameter[] originalParameters = new IDataParameter[parameters.Count];
      parameters.CopyTo((Array) originalParameters, 0);
      return CachingMechanism.CloneParameters(originalParameters);
    }
  }
}
