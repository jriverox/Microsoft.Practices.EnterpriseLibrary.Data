// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.DatabaseConnectionWrapper
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.Data.Common;
using System.Threading;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class DatabaseConnectionWrapper : IDisposable
  {
    private int refCount;

    public DatabaseConnectionWrapper(DbConnection connection)
    {
      this.Connection = connection;
      this.refCount = 1;
    }

    public DbConnection Connection { get; private set; }

    public bool IsDisposed
    {
      get
      {
        return this.refCount == 0;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || Interlocked.Decrement(ref this.refCount) != 0)
        return;
      this.Connection.Dispose();
      this.Connection = (DbConnection) null;
      GC.SuppressFinalize((object) this);
    }

    public DatabaseConnectionWrapper AddRef()
    {
      Interlocked.Increment(ref this.refCount);
      return this;
    }
  }
}
