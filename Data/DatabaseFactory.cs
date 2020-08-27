// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public static class DatabaseFactory
  {
    public static Database CreateDatabase()
    {
      return DatabaseFactory.InnerCreateDatabase((string) null);
    }

    public static Database CreateDatabase(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, name);
      return DatabaseFactory.InnerCreateDatabase(name);
    }

    private static Database InnerCreateDatabase(string name)
    {
      try
      {
        return EnterpriseLibraryContainer.Current.GetInstance<Database>(name);
      }
      catch (ActivationException ex)
      {
        DatabaseFactory.TryLogConfigurationError((Exception) ex, name);
        throw;
      }
    }

    private static void TryLogConfigurationError(
      Exception configurationException,
      string instanceName)
    {
      try
      {
        EnterpriseLibraryContainer.Current.GetInstance<DefaultDataEventLogger>()?.LogConfigurationError(configurationException, instanceName);
      }
      catch
      {
      }
    }
  }
}
