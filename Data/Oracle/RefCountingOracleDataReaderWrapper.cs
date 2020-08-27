// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.RefCountingOracleDataReaderWrapper
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System.Data.OracleClient;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle
{
  public class RefCountingOracleDataReaderWrapper : OracleDataReaderWrapper
  {
    private readonly DatabaseConnectionWrapper connection;

    internal RefCountingOracleDataReaderWrapper(
      DatabaseConnectionWrapper connection,
      OracleDataReader innerReader)
      : base(innerReader)
    {
      this.connection = connection;
      this.connection.AddRef();
    }

    public override void Close()
    {
      if (this.IsClosed)
        return;
      base.Close();
      this.connection.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.IsClosed)
        return;
      base.Dispose(true);
      this.connection.Dispose();
    }
  }
}
