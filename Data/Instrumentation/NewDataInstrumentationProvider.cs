// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation.NewDataInstrumentationProvider
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
  [HasInstallableResources]
  [EventLogDefinition("Application", "Enterprise Library Data")]
  [PerformanceCountersDefinition("Enterprise Library Data Counters", "CounterCategoryHelpResourceName")]
  public class NewDataInstrumentationProvider : InstrumentationListener, IDataInstrumentationProvider
  {
    private static readonly EnterpriseLibraryPerformanceCounterFactory counterCache = new EnterpriseLibraryPerformanceCounterFactory();
    public const string CounterCategoryName = "Enterprise Library Data Counters";
    public const string TotalConnectionOpenedCounter = "Total Connections Opened";
    public const string TotalConnectionFailedCounter = "Total Connections Failed";
    public const string TotalCommandsExecutedCounter = "Total Commands Executed";
    public const string TotalCommandsFailedCounter = "Total Commands Failed";
    [PerformanceCounter("Connections Opened/sec", "ConnectionOpenedCounterHelpResource", PerformanceCounterType.RateOfCountsPerSecond32)]
    private EnterpriseLibraryPerformanceCounter connectionOpenedCounter;
    [PerformanceCounter("Total Connections Opened", "TotalConnectionOpenedHelpResource", PerformanceCounterType.NumberOfItems32)]
    private EnterpriseLibraryPerformanceCounter totalConnectionOpenedCounter;
    [PerformanceCounter("Commands Executed/sec", "CommandExecutedCounterHelpResource", PerformanceCounterType.RateOfCountsPerSecond32)]
    private EnterpriseLibraryPerformanceCounter commandExecutedCounter;
    [PerformanceCounter("Total Commands Executed", "TotalCommandsExecutedHelpResource", PerformanceCounterType.NumberOfItems32)]
    private EnterpriseLibraryPerformanceCounter totalCommandsExecutedCounter;
    [PerformanceCounter("Connections Failed/sec", "ConnectionFailedCounterHelpResource", PerformanceCounterType.RateOfCountsPerSecond32)]
    private EnterpriseLibraryPerformanceCounter connectionFailedCounter;
    [PerformanceCounter("Total Connections Failed", "TotalConnectionFailedHelpResource", PerformanceCounterType.NumberOfItems32)]
    private EnterpriseLibraryPerformanceCounter totalConnectionFailedCounter;
    [PerformanceCounter("Commands Failed/sec", "CommandFailedCounterHelpResource", PerformanceCounterType.RateOfCountsPerSecond32)]
    private EnterpriseLibraryPerformanceCounter commandFailedCounter;
    [PerformanceCounter("Total Commands Failed", "TotalCommandsFailedHelpResource", PerformanceCounterType.NumberOfItems32)]
    private EnterpriseLibraryPerformanceCounter totalCommandsFailedCounter;
    private readonly string instanceName;

    public NewDataInstrumentationProvider(
      string instanceName,
      bool performanceCountersEnabled,
      bool eventLoggingEnabled,
      string applicationInstanceName)
      : this(instanceName, performanceCountersEnabled, eventLoggingEnabled, (IPerformanceCounterNameFormatter) new AppDomainNameFormatter(applicationInstanceName))
    {
    }

    public NewDataInstrumentationProvider(
      string instanceName,
      bool performanceCountersEnabled,
      bool eventLoggingEnabled,
      IPerformanceCounterNameFormatter nameFormatter)
      : base(instanceName, performanceCountersEnabled, eventLoggingEnabled, nameFormatter)
    {
      this.instanceName = instanceName;
    }

    public void FireConnectionOpenedEvent()
    {
      if (!this.PerformanceCountersEnabled)
        return;
      this.connectionOpenedCounter.Increment();
      this.totalConnectionOpenedCounter.Increment();
    }

    public void FireCommandExecutedEvent(DateTime startTime)
    {
      if (!this.PerformanceCountersEnabled)
        return;
      this.commandExecutedCounter.Increment();
      this.totalCommandsExecutedCounter.Increment();
    }

    public void FireConnectionFailedEvent(string connectionString, Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      if (this.PerformanceCountersEnabled)
      {
        this.connectionFailedCounter.Increment();
        this.totalConnectionFailedCounter.Increment();
      }
      if (!this.EventLoggingEnabled)
        return;
      string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ErrorConnectionFailedMessage, (object) this.instanceName);
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ErrorConnectionFailedExtraInformation, (object) connectionString);
      EventLog.WriteEntry(this.GetEventSourceName(), new EventLogEntryFormatter(Resources.BlockName).GetEntryText(message, exception, str), EventLogEntryType.Error);
    }

    public void FireCommandFailedEvent(
      string commandText,
      string connectionString,
      Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      if (!this.PerformanceCountersEnabled)
        return;
      this.commandFailedCounter.Increment();
      this.totalCommandsFailedCounter.Increment();
    }

    public static void ClearCounterCache()
    {
      NewDataInstrumentationProvider.counterCache.ClearCachedCounters();
    }

    protected override void CreatePerformanceCounters(string[] instanceNames)
    {
      this.connectionOpenedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Connections Opened/sec", instanceNames);
      this.commandExecutedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Commands Executed/sec", instanceNames);
      this.connectionFailedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Connections Failed/sec", instanceNames);
      this.commandFailedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Commands Failed/sec", instanceNames);
      this.totalConnectionOpenedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Total Connections Opened", instanceNames);
      this.totalConnectionFailedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Total Connections Failed", instanceNames);
      this.totalCommandsExecutedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Total Commands Executed", instanceNames);
      this.totalCommandsFailedCounter = NewDataInstrumentationProvider.counterCache.CreateCounter("Enterprise Library Data Counters", "Total Commands Failed", instanceNames);
    }
  }
}
