// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.RefCountingDataReader
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.Unity.Utility;
using System;
using System.Data;
using System.Data.Common;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class RefCountingDataReaderForAsync : DataReaderWrapperForAsync
    {
    private readonly DatabaseConnectionWrapper connectionWrapper;

    public RefCountingDataReaderForAsync(DatabaseConnectionWrapper connection, DbDataReader innerReader)
      : base(innerReader)
    {
      Guard.ArgumentNotNull((object) connection, nameof (connection));
      Guard.ArgumentNotNull((object) innerReader, nameof (innerReader));
      this.connectionWrapper = connection;
      this.connectionWrapper.AddRef();
    }

    public static explicit operator DbDataReader(RefCountingDataReaderForAsync wrapper)
    {
        if (wrapper == null || wrapper.InnerReader == null)
            throw new ArgumentNullException();
        return wrapper.InnerReader;
    }
    public override void Close()
    {
      if (this.IsClosed)
        return;
      base.Close();
      this.connectionWrapper.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.IsClosed)
        return;
      base.Dispose(true);
      this.connectionWrapper.Dispose();
    }
  }
}
