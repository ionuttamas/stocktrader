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
// Special class for running BSL in-process with the Web app.
//======================================================================================================


using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceNodeCommunication.DataContract;
using ConfigService.ServiceConfigurationUtility;
using Trade.StockTraderWebApplicationSettings;
using Trade.BusinessServiceConfigurationSettings;
using Trade.BusinessServiceHostConfigurationImplementation;
using Trade.Utility;

namespace Trade.StockTraderWebApplicationConfigurationImplementation
{
    /// <summary>
    /// Class to initialize Business Services when running in-process with Web app: AccessMode="InProcess"".
    /// </summary>
    public static class InitConfigInProcessBusinessService
    {
        public static bool initialized = false;
        /// <summary>
        /// Loads BSL config settings from its repository, performs init on in-process Business Services.
        /// </summary>
        public static void initConfigBusinessService(object lockobject, bool force)
        {
            lock (lockobject)
            {
                if (!force && initialized)
                    return;
                initialized = true;
                ConnectionStringSettings connSettingOPS = System.Configuration.ConfigurationManager.ConnectionStrings["OPS_CONFIGDB_SQL_CONN_STRING"];
                ConnectionStringSettings connSettingBSL = System.Configuration.ConfigurationManager.ConnectionStrings["BSL_CONFIGDB_SQL_CONN_STRING"];
                StockTraderWebApplicationSettings.Settings.BSL_CONFIGDB_SQL_CONN_STRING = connSettingBSL.ConnectionString;
                StockTraderWebApplicationSettings.Settings.OPS_CONFIGDB_SQL_CONN_STRING = connSettingOPS.ConnectionString;
                Trade.BusinessServiceConfigurationSettings.Settings.CONFIGDB_SQL_CONN_STRING = StockTraderWebApplicationSettings.Settings.BSL_CONFIGDB_SQL_CONN_STRING;
                Trade.BusinessServiceConfigurationSettings.Settings BSLSettings = new Trade.BusinessServiceConfigurationSettings.Settings();
                ServiceConfigHelper configHelper = new ServiceConfigHelper(BSLSettings);
                Trade.BusinessServiceHostConfigurationImplementation.ConfigurationActions BSLConfigActions = new Trade.BusinessServiceHostConfigurationImplementation.ConfigurationActions();
                Trade.BusinessServiceConfigurationSettings.Settings.thisService = new Trade.BusinessServiceHostConfigurationImplementation.ConfigurationService();
                Trade.BusinessServiceConfigurationSettings.Settings.thisServiceConfigActions = BSLConfigActions;
                configHelper.InitializeConfigFromDatabase(false, ref Trade.BusinessServiceConfigurationSettings.Settings.connectionPointList, ref Trade.BusinessServiceConfigurationSettings.Settings.connectedServices, ref Trade.BusinessServiceConfigurationSettings.Settings.connectedConfigServices, ref Trade.BusinessServiceConfigurationSettings.Settings.hostedServices, ref Trade.BusinessServiceConfigurationSettings.Settings.serviceHosts, Trade.BusinessServiceConfigurationSettings.Settings.CONFIGDB_SQL_CONN_STRING);
                BSLConfigActions.initBusinessService(BSLSettings);
                if (Trade.BusinessServiceConfigurationSettings.Settings.ORDER_PROCESSING_MODE == StockTraderUtility.OPS_INPROCESS)
                {
                    Trade.OrderProcessorServiceConfigurationSettings.Settings.CONFIGDB_SQL_CONN_STRING = StockTraderWebApplicationSettings.Settings.OPS_CONFIGDB_SQL_CONN_STRING;
                    string thelockobject = "0";
                    Trade.BusinessServiceHostConfigurationImplementation.InitConfigInProcessOrderService.initConfigOrderProcessService(thelockobject,false);
                }
                RemoteNotifications remoteNotify = new RemoteNotifications(new Trade.BusinessServiceConfigurationSettings.Settings());
                remoteNotify.notifyRemoteClientsAndHosts(false, ConfigUtility.ADD_HOSTS);
                List<ConnectedServices> connectedServices = (List<ConnectedServices>)ConfigUtility.reflectGetField(BSLSettings, "connectedServices");
                object BSLSettingsObject = (object)BSLSettings;
                if (ServiceConfigHelper.MasterServiceWebHost.MasterHost != null)
                {
                    ServiceConfigHelper.initOnlineMethodsCS(connectedServices, ServiceConfigHelper.MasterServiceWebHost.MasterHost._connectedServiceContracts, ref BSLSettingsObject);
                }
                else
                    if (ServiceConfigHelper.MasterServiceSelfHost.MasterHost != null)
                    {
                        ServiceConfigHelper.initOnlineMethodsCS(connectedServices, ServiceConfigHelper.MasterServiceSelfHost.MasterHost._connectedServiceContracts, ref BSLSettingsObject);
                    }

                BSLConfigActions.InitCommunicationChannels(BSLSettings, null, ConfigUtility.INIT_ALL_CONNECTED_INSTANCES);
            }
        }
   }
}
