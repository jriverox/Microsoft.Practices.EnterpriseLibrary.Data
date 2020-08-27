// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation.DefaultDataEventLogger
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Properties;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation
{
  [EventLogDefinition("Application", "Enterprise Library Data")]
  public class DefaultDataEventLogger : InstrumentationListener
  {
    private readonly IEventLogEntryFormatter eventLogEntryFormatter;

    public DefaultDataEventLogger(bool eventLoggingEnabled)
      : base((string) null, false, eventLoggingEnabled, (IPerformanceCounterNameFormatter) null)
    {
      this.eventLogEntryFormatter = (IEventLogEntryFormatter) new EventLogEntryFormatter(Resources.BlockName);
    }

    public void LogConfigurationError(Exception exception, string instanceName)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      if (!this.EventLoggingEnabled)
        return;
      EventLog.WriteEntry(this.GetEventSourceName(), this.eventLogEntryFormatter.GetEntryText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ConfigurationFailureCreatingDatabase, (object) instanceName), exception), EventLogEntryType.Error);
    }
  }
}
