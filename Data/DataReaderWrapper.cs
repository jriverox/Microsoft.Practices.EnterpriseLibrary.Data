// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.DataReaderWrapper
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System;
using System.Data;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public abstract class DataReaderWrapper : MarshalByRefObject, IDataReader, IDisposable, IDataRecord
  {
    private readonly IDataReader innerReader;

    protected DataReaderWrapper(IDataReader innerReader)
    {
      this.innerReader = innerReader;
    }

    public IDataReader InnerReader
    {
      get
      {
        return this.innerReader;
      }
    }

    public virtual int FieldCount
    {
      get
      {
        return this.innerReader.FieldCount;
      }
    }

    public virtual int Depth
    {
      get
      {
        return this.innerReader.Depth;
      }
    }

    public virtual bool IsClosed
    {
      get
      {
        return this.innerReader.IsClosed;
      }
    }

    public virtual int RecordsAffected
    {
      get
      {
        return this.innerReader.RecordsAffected;
      }
    }

    public virtual void Close()
    {
      if (this.innerReader.IsClosed)
        return;
      this.innerReader.Close();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.innerReader.IsClosed)
        return;
      this.innerReader.Dispose();
    }

    public virtual string GetName(int i)
    {
      return this.innerReader.GetName(i);
    }

    public virtual string GetDataTypeName(int i)
    {
      return this.innerReader.GetDataTypeName(i);
    }

    public virtual Type GetFieldType(int i)
    {
      return this.innerReader.GetFieldType(i);
    }

    public virtual object GetValue(int i)
    {
      return this.innerReader.GetValue(i);
    }

    public virtual int GetValues(object[] values)
    {
      return this.innerReader.GetValues(values);
    }

    public virtual int GetOrdinal(string name)
    {
      return this.innerReader.GetOrdinal(name);
    }

    public virtual bool GetBoolean(int i)
    {
      return this.innerReader.GetBoolean(i);
    }

    public virtual byte GetByte(int i)
    {
      return this.innerReader.GetByte(i);
    }

    public virtual long GetBytes(
      int i,
      long fieldOffset,
      byte[] buffer,
      int bufferoffset,
      int length)
    {
      return this.innerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
    }

    public virtual char GetChar(int i)
    {
      return this.innerReader.GetChar(i);
    }

    public virtual long GetChars(
      int i,
      long fieldoffset,
      char[] buffer,
      int bufferoffset,
      int length)
    {
      return this.innerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
    }

    public virtual Guid GetGuid(int i)
    {
      return this.innerReader.GetGuid(i);
    }

    public virtual short GetInt16(int i)
    {
      return this.innerReader.GetInt16(i);
    }

    public virtual int GetInt32(int i)
    {
      return this.innerReader.GetInt32(i);
    }

    public virtual long GetInt64(int i)
    {
      return this.innerReader.GetInt64(i);
    }

    public virtual float GetFloat(int i)
    {
      return this.innerReader.GetFloat(i);
    }

    public virtual double GetDouble(int i)
    {
      return this.innerReader.GetDouble(i);
    }

    public virtual string GetString(int i)
    {
      return this.innerReader.GetString(i);
    }

    public virtual Decimal GetDecimal(int i)
    {
      return this.innerReader.GetDecimal(i);
    }

    public virtual DateTime GetDateTime(int i)
    {
      return this.innerReader.GetDateTime(i);
    }

    public virtual IDataReader GetData(int i)
    {
      return this.innerReader.GetData(i);
    }

    public virtual bool IsDBNull(int i)
    {
      return this.innerReader.IsDBNull(i);
    }

    object IDataRecord.this[int i]
    {
      get
      {
        return this.innerReader[i];
      }
    }

    object IDataRecord.this[string name]
    {
      get
      {
        return this.innerReader[name];
      }
    }

    public virtual DataTable GetSchemaTable()
    {
      return this.innerReader.GetSchemaTable();
    }

    public virtual bool NextResult()
    {
      return this.innerReader.NextResult();
    }

    public virtual bool Read()
    {
      return this.innerReader.Read();
    }
  }
}
