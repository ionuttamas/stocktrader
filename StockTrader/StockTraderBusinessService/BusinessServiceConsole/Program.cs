//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   4/10/2011: Updated to version 5.0, with notable enhancements for optional Windows Azure hosting, cross-browser and mobile-browser compatibility, and 
//                   new performance ehancements  See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//                                   
//
//  Configuration Service 5 Notes:
//                      The application implements Configuration Service 5.0, for which the source code also ships in the sample. However, the .NET StockTrader 5
//                      sample is a general-purpose performance sample application for Windows Server and Windows Azure even if you are not implementing the Configuration Service. 
//                      
//

//======================================================================================================
// This is one of three optional host programs for the Business Service Layer.  In this case a Console  
// application.  You can also optionally run the Windows host, as well as the IIS-hosted implementation.  
//======================================================================================================



using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceNodeCommunicationImplementation;
using ConfigService.ServiceNodeCommunication.DataContract;
using ConfigService.ServiceConfigurationUtility;
using ConfigService.RuntimeHostData;
using ConfigService.ServiceHostShellConsoleBase;
using Trade.BusinessServiceConfigurationSettings;
using Trade.BusinessServiceHostConfigurationImplementation;
using Trade.BusinessServiceImplementation;
using Trade.OrderProcessorContract;


namespace Trade.BusinessServiceConsole
{
    
    class BusinessService_ConsoleHost
    {
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
                ConfigUtility.setAzureRuntime(false);
                //The key call to create our list of runtime hosts to be initialized.
                List<ServiceHostInfo> startupList = new List<ServiceHostInfo>(new ServiceHostInfo[] { new ServiceHostInfo(false, null, new object[] { new Trade.BusinessServiceImplementation.ErrorBehaviorAttribute() }, new TradeServiceBSL()) });
                //Stock call to startup the Master Host.
                base.startService(new Settings(), new ConfigurationService(), new NodeCommunication(), null, new ConfigurationActions(true), startupList, null, new object[] { typeof(IOrderProcessor) });
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth - 30, Console.LargestWindowHeight - 30);
            Console.Title = ".NET StockTrader Business Services Host";
            MyHost myHost = new MyHost();
            myHost.startUp();
        }
    }
}
