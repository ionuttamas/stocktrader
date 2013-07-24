//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//                                   
//
//  Configuration Service 6 Notes:
//                      The application implements Configuration Service 6.0, for which the source code also ships in the sample. However, the .NET StockTrader 6
//                      sample is a general-purpose performance sample application for Windows Server and Windows Azure even if you are not implementing the Configuration Service. 
//                      
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.ServiceModel.Description;
using System.Threading;
using ConfigService.AzureConfigUtility;
using ConfigService.RuntimeHostData;
using ConfigService.ServiceConfigurationUtility;
using ConfigService.ServiceHostShellConsoleBase;
using ConfigService.ServiceNodeCommunicationImplementation;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorHostConfigurationImplementation;
using Trade.OrderProcessorImplementation;
using Trade.OrderProcessorServiceConfigurationSettings;

namespace AzureOPSRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private static MyHost myHost;
        private static EndPointBehaviors endpointBehaviorList;
        public static List<ServiceHostInfo> startupList;

        /// <summary>
        /// The program entry class. Note how this simply inherits from the provided base class.
        /// </summary>
        class MyHost : ShellServiceConsoleBase
        {
            /// <summary>
            /// This is the key call where you will define parameters for the Master host startup, and call
            /// the base 'startService' method.
            /// </summary>
            public void startUp()
            {
                ConfigUtility.setAzureRuntime(true);
                //First we will create a list to hold our runtime-created endpoint behavior.  Note we could also simply
                //use config to define, and select via ConfigWeb on the endpoint (HostedService) definition, this is an
                //alternate mechanism to provide via code as an example.
                List<object> endpointBehaviors = new List<object>();
                //Now we will construct (and add to our list) an endpoint behavior, in this case a TransactedBatchingBehavior.  This behavior
                //will be applied to the MSMQ endpoint, and allows .NET to process multiple entries from the queue as part of
                //a single distributed transaction---a performance benefit that must be balanced against potential database concurrency issues
                //if set too high.
                endpointBehaviors.Add(new TransactedBatchingBehavior(5));
                endpointBehaviorList = new EndPointBehaviors(endpointBehaviors, null);
                //Now the key call to create our list of runtime hosts to be initialized.
                startupList = new List<ServiceHostInfo>(new[]
                                                            {
                                                                new ServiceHostInfo(false, null, new object[] { new ErrorBehaviorAttribute() }, new OrderProcessor()),
                                                                new ServiceHostInfo(false, null, new object[] { new ErrorBehaviorAttribute() }, new QuoteService())
                                                            });
                //Stock call to startup the Master Host.
                base.startNTService(new Settings(), new ConfigurationService(), new NodeCommunication(), null, new ConfigurationActions(), startupList, endpointBehaviorList, new object[] { typeof(IOrderProcessorResponse) });
            }
        }

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            //Trace.WriteLine("Azure Order Processor entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        //Event Handler
        public void RoleInstanceShutdown(object sender, RoleEnvironmentStoppingEventArgs e)
        {
            myHost.deActivateHosts();
            ConfigUtility.writeConsoleMessage("\nWorker Role RoleInstanceShutdown: Node ID: " + AzureUtility.getRoleInstanceID() + " Has Shut Down. Goodbye!\n", EventLogEntryType.Warning, true, new Settings());
        }

        //Event Handler
        public void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            /*var changes = from ch in e.Changes.OfType<RoleEnvironmentTopologyChange>()
                          select ch;
            if (changes.Any())
            {
                myHost.deActivateHosts();
                ConfigUtility.writeConsoleMessage("\nWorker Role OnStop: Node ID: " + AzureUtility.getRoleInstanceID() + " Has Shut Down. Goodbye!\n", EventLogEntryType.Warning, true, new Sample.HelloServiceSettings.Settings()); 
            }*/
        }

        public override bool OnStart()
        {
            ConfigureWindowsAzureDiagnostics();

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 64;

            //Wire up our key event handlers, to ensure proper lifecycle management.
            RoleEnvironment.Stopping += RoleInstanceShutdown;
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            myHost = new MyHost();
            myHost.startUp();

            return base.OnStart();
        }

        public override void OnStop()
        {
            Thread.Sleep(22000);
            base.OnStop();
        }

        private static void ConfigureWindowsAzureDiagnostics()
        {
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();

            // Configure WCF tracing
            //config.Directories.DataSources.Add(
            //    new DirectoryConfiguration
            //    {
            //        Path = RoleEnvironment.GetLocalResource("OpsWcf.svclog").RootPath,
            //        Container = "wad-tracefiles",
            //        DirectoryQuotaInMB = 1000
            //    });
            //config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(10);

            // Record event log errors
            config.WindowsEventLog.DataSources.Add("Application!*[System[(Level=1 or Level=2)]]");
            config.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);
        }
    }
}
