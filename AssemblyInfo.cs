using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability;
using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;

[assembly: ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
[assembly: AssemblyTitle("Enterprise Library Data Access Application Block")]
//[assembly: Extension]
[assembly: ConfigurationSectionManageabilityProvider("connectionStrings", typeof (ConnectionStringsManageabilityProvider))]
[assembly: AssemblyDescription("Enterprise Library Data Access Application Block")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: HandlesSection("connectionStrings")]
[assembly: HandlesSection("dataConfiguration", ClearOnly = true)]
[assembly: AddApplicationBlockCommand("connectionStrings", typeof (ConnectionStringsSection), CommandModelTypeName = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.AddDatabaseBlockCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime", TitleResourceName = "AddDataSettings", TitleResourceType = typeof (DesignResources))]
[assembly: AssemblyProduct("Microsoft Enterprise Library for .NET")]
[assembly: AssemblyTrademark("")]
[assembly: ConfigurationSectionManageabilityProvider("oracleConnectionSettings", typeof (OracleConnectionSettingsManageabilityProvider))]
[assembly: CLSCompliant(true)]
[assembly: ConfigurationSectionManageabilityProvider("dataConfiguration", typeof (DatabaseSettingsManageabilityProvider))]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyVersion("5.1.0.0")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
