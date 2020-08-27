// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Oracle.OracleDataReaderWrapper
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.Collections;
using System.Data;
using System.Data.OracleClient;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Oracle
{
  public class OracleDataReaderWrapper : DataReaderWrapper, IEnumerable
  {
    internal OracleDataReaderWrapper(OracleDataReader innerReader)
      : base((IDataReader) innerReader)
    {
    }

    public OracleDataReader InnerReader
    {
      get
      {
        return (OracleDataReader) base.InnerReader;
      }
    }

    public override bool GetBoolean(int index)
    {
      return Convert.ToBoolean(this.InnerReader[index], (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public override byte GetByte(int index)
    {
      return Convert.ToByte(this.InnerReader[index], (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public override Guid GetGuid(int index)
    {
      return new Guid((byte[]) this.InnerReader[index]);
    }

    public override short GetInt16(int index)
    {
      return Convert.ToInt16(this.InnerReader[index], (IFormatProvider) CultureInfo.InvariantCulture);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.InnerReader.GetEnumerator();
    }
  }
}
