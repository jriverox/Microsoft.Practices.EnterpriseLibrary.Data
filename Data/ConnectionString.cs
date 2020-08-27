// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.ConnectionString
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Text;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public class ConnectionString
  {
    private const char CONNSTRING_DELIM = ';';
    private string connectionString;
    private string connectionStringWithoutCredentials;
    private string userIdTokens;
    private string passwordTokens;

    public ConnectionString(string connectionString, string userIdTokens, string passwordTokens)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (connectionString));
      if (string.IsNullOrEmpty(userIdTokens))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (userIdTokens));
      if (string.IsNullOrEmpty(passwordTokens))
        throw new ArgumentException(Resources.ExceptionNullOrEmptyString, nameof (passwordTokens));
      this.connectionString = connectionString;
      this.userIdTokens = userIdTokens;
      this.passwordTokens = passwordTokens;
      this.connectionStringWithoutCredentials = (string) null;
    }

    public string UserName
    {
      get
      {
        string lowerInvariant = this.connectionString.ToLowerInvariant();
        int tokenPos;
        int tokenMPos;
        this.GetTokenPositions(this.userIdTokens, out tokenPos, out tokenMPos);
        if (0 > tokenPos)
          return string.Empty;
        int num = lowerInvariant.IndexOf(';', tokenMPos);
        return this.connectionString.Substring(tokenMPos, num - tokenMPos);
      }
      set
      {
        string lowerInvariant = this.connectionString.ToLowerInvariant();
        int tokenPos;
        int tokenMPos;
        this.GetTokenPositions(this.userIdTokens, out tokenPos, out tokenMPos);
        if (0 <= tokenPos)
        {
          int startIndex = lowerInvariant.IndexOf(';', tokenMPos);
          this.connectionString = this.connectionString.Substring(0, tokenMPos) + value + this.connectionString.Substring(startIndex);
        }
        else
        {
          string[] strArray = this.userIdTokens.Split(',');
          ConnectionString connectionString = this;
          connectionString.connectionString = connectionString.connectionString + strArray[0] + value + (object) ';';
        }
      }
    }

    public string Password
    {
      get
      {
        string lowerInvariant = this.connectionString.ToLowerInvariant();
        int tokenPos;
        int tokenMPos;
        this.GetTokenPositions(this.passwordTokens, out tokenPos, out tokenMPos);
        if (0 > tokenPos)
          return string.Empty;
        int num = lowerInvariant.IndexOf(';', tokenMPos);
        return this.connectionString.Substring(tokenMPos, num - tokenMPos);
      }
      set
      {
        string lowerInvariant = this.connectionString.ToLowerInvariant();
        int tokenPos;
        int tokenMPos;
        this.GetTokenPositions(this.passwordTokens, out tokenPos, out tokenMPos);
        if (0 <= tokenPos)
        {
          int startIndex = lowerInvariant.IndexOf(';', tokenMPos);
          this.connectionString = this.connectionString.Substring(0, tokenMPos) + value + this.connectionString.Substring(startIndex);
        }
        else
        {
          string[] strArray = this.passwordTokens.Split(',');
          ConnectionString connectionString = this;
          connectionString.connectionString = connectionString.connectionString + strArray[0] + value + (object) ';';
        }
      }
    }

    public override string ToString()
    {
      return this.connectionString;
    }

    public string ToStringNoCredentials()
    {
      if (this.connectionStringWithoutCredentials == null)
        this.connectionStringWithoutCredentials = this.RemoveCredentials(this.connectionString);
      return this.connectionStringWithoutCredentials;
    }

    public ConnectionString CreateNewConnectionString(
      string connectionStringToFormat)
    {
      return new ConnectionString(connectionStringToFormat, this.userIdTokens, this.passwordTokens);
    }

    private void GetTokenPositions(string tokenString, out int tokenPos, out int tokenMPos)
    {
      string[] strArray = tokenString.Split(',');
      int num1 = -1;
      string lowerInvariant = this.connectionString.ToLowerInvariant();
      tokenPos = -1;
      tokenMPos = -1;
      foreach (string str in strArray)
      {
        int num2 = lowerInvariant.IndexOf(str);
        if (num2 > num1)
        {
          tokenPos = num2;
          tokenMPos = num2 + str.Length;
          num1 = num2;
        }
      }
    }

    private string RemoveCredentials(string connectionStringToModify)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray1 = connectionStringToModify.ToLowerInvariant().Split(';');
      string[] strArray2 = (this.userIdTokens + "," + this.passwordTokens).ToLowerInvariant().Split(',');
      foreach (string str1 in strArray1)
      {
        bool flag = false;
        string str2 = str1.Trim();
        if (str2.Length != 0)
        {
          foreach (string str3 in strArray2)
          {
            if (str2.StartsWith(str3))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            stringBuilder.Append(str2 + (object) ';');
        }
      }
      return stringBuilder.ToString();
    }
  }
}
