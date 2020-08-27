// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation.DataInstrumentationInstaller
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using System.ComponentModel;
using System.Configuration.Install;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation
{
  [RunInstaller(true)]
  public class DataInstrumentationInstaller : Installer
  {
    private IContainer components;

    public DataInstrumentationInstaller()
    {
      this.Installers.Add((Installer) new ReflectionInstaller<PerformanceCounterInstallerBuilder>());
      this.Installers.Add((Installer) new ReflectionInstaller<EventLogInstallerBuilder>());
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
    }
  }
}
