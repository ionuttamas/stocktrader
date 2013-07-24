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

//======================================================================================================
// Special class for running Order Processor Service in-process with the BSL.
//======================================================================================================

using System.Collections.Generic;
using System.Configuration;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceConfigurationUtility;
using ConfigService.ServiceNodeCommunication.DataContract;

namespace Trade.BusinessServiceHostConfigurationImplementation
{
    /// <summary>
    /// Class to initialize Order Processor Service when running in-process with Web app: OrderMode="Sync-InProcess".
    /// </summary>
   public static class InitConfigInProcessOrderService
    {
       public static bool initialized = false;
        /// <summary>
        /// Loads OPS config settings from its repository, performs init on in-process Order Processor Service.
        /// </summary>
       public static void initConfigOrderProcessService(object lockobject, bool force)
       {
           lock (lockobject)
           {
               if (!force && initialized)
                   return;
               initialized = true;
               ConnectionStringSettings connSettingOPS = System.Configuration.ConfigurationManager.ConnectionStrings["OPS_CONFIGDB_SQL_CONN_STRING"];
               Trade.OrderProcessorServiceConfigurationSettings.Settings.CONFIGDB_SQL_CONN_STRING = connSettingOPS.ConnectionString;
               Trade.OrderProcessorServiceConfigurationSettings.Settings OPSSettings = new Trade.OrderProcessorServiceConfigurationSettings.Settings();
               ServiceConfigHelper configHelper = new ServiceConfigHelper(OPSSettings);
               Trade.OrderProcessorHostConfigurationImplementation.ConfigurationActions OPSConfigActions = new Trade.OrderProcessorHostConfigurationImplementation.ConfigurationActions();
               Trade.OrderProcessorServiceConfigurationSettings.Settings.thisService = new Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService();
               Trade.OrderProcessorServiceConfigurationSettings.Settings.thisServiceConfigActions = OPSConfigActions;
               configHelper.InitializeConfigFromDatabase(false, ref Trade.OrderProcessorServiceConfigurationSettings.Settings.connectionPointList, ref Trade.OrderProcessorServiceConfigurationSettings.Settings.connectedServices, ref Trade.OrderProcessorServiceConfigurationSettings.Settings.connectedConfigServices, ref Trade.OrderProcessorServiceConfigurationSettings.Settings.hostedServices, ref Trade.OrderProcessorServiceConfigurationSettings.Settings.serviceHosts, Trade.OrderProcessorServiceConfigurationSettings.Settings.CONFIGDB_SQL_CONN_STRING);
               OPSConfigActions.initOrderForwardMode();
               List<ConnectedServices> connectedServices = (List<ConnectedServices>)ConfigUtility.reflectGetField(OPSSettings, "connectedServices");
               object OPSSettingsObject = (object)OPSSettings;
               if (ServiceConfigHelper.MasterServiceWebHost.MasterHost != null)
               {
                   ServiceConfigHelper.initOnlineMethodsCS(connectedServices, ServiceConfigHelper.MasterServiceWebHost.MasterHost._connectedServiceContracts, ref OPSSettingsObject);
               }
               else
                   if (ServiceConfigHelper.MasterServiceSelfHost.MasterHost != null)
                   {
                       ServiceConfigHelper.initOnlineMethodsCS(connectedServices, ServiceConfigHelper.MasterServiceSelfHost.MasterHost._connectedServiceContracts, ref OPSSettingsObject);
                   }
               OPSConfigActions.InitCommunicationChannels(OPSSettings, null, ConfigUtility.INIT_ALL_CONNECTED_INSTANCES);
           }
       }
    }
}
