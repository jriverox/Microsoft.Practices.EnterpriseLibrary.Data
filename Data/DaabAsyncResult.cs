// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.DaabAsyncResult
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.Data.Common;
using System.Threading;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class DaabAsyncResult : IAsyncResult
  {
    private readonly IAsyncResult innerAsyncResult;
    private readonly DbCommand command;
    private readonly bool disposeCommand;
    private readonly bool closeConnection;
    private readonly DateTime startTime;

    public DaabAsyncResult(
      IAsyncResult innerAsyncResult,
      DbCommand command,
      bool disposeCommand,
      bool closeConnection,
      DateTime startTime)
    {
      this.innerAsyncResult = innerAsyncResult;
      this.command = command;
      this.disposeCommand = disposeCommand;
      this.closeConnection = closeConnection;
      this.startTime = startTime;
    }

    public object AsyncState
    {
      get
      {
        return this.innerAsyncResult.AsyncState;
      }
    }

    public WaitHandle AsyncWaitHandle
    {
      get
      {
        return this.innerAsyncResult.AsyncWaitHandle;
      }
    }

    public bool CompletedSynchronously
    {
      get
      {
        return this.innerAsyncResult.CompletedSynchronously;
      }
    }

    public bool IsCompleted
    {
      get
      {
        return this.innerAsyncResult.IsCompleted;
      }
    }

    public IAsyncResult InnerAsyncResult
    {
      get
      {
        return this.innerAsyncResult;
      }
    }

    public bool DisposeCommand
    {
      get
      {
        return this.disposeCommand;
      }
    }

    public DbCommand Command
    {
      get
      {
        return this.command;
      }
    }

    public bool CloseConnection
    {
      get
      {
        return this.closeConnection;
      }
    }

    public DbConnection Connection
    {
      get
      {
        return this.command.Connection;
      }
    }

    public DateTime StartTime
    {
      get
      {
        return this.startTime;
      }
    }
  }
}
