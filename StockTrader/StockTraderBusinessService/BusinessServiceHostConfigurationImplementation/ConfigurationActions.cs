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
// The Business Services ConfigurationActions class, defining actions that get executed on Configuration 
// Updates.
//======================================================================================================


using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Security;
using System.Configuration;
using System.Diagnostics;
using System.Data.SqlClient;
using ConfigService.ServiceConfigurationBase;
using ConfigService.ServiceConfiguration.DataContract;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceConfigurationUtility;
using ConfigService.RuntimeHostData;
using ConfigService.ServiceNodeCommunication.DataContract;
using ConfigService.ServiceNodeCommunicationImplementation;
using Trade.OrderProcessorContract;
using Trade.BusinessServiceImplementation;
using Trade.BusinessServiceConfigurationSettings;
using Trade.BusinessServiceContract;
using Trade.OrderProcessorAsyncClient;
using Trade.Utility;

namespace Trade.BusinessServiceHostConfigurationImplementation 
{
    /// <summary>
    /// This class is a core part of an implementation of the configuration management service/system.  
    /// This class should inherit from the base implementation ConfigurationActiponsBase. The class,
    /// including methods defined in the base, performs configuration operations (actions) required to
    /// maintain the live running configuration system.
    /// </summary>
    public class ConfigurationActions : ConfigurationActionsBase
    {
        /// <summary>
        /// Constructor, which is also an entry point into creating the IIS-host MasterServiceWebHost and
        /// associated WCF ServiceHosts.  
        /// </summary>
        public ConfigurationActions()
        {
            lock (ConfigUtility.masterServiceWebHostSyncObject)
            {
                List<ServiceHostInfo> startupList = null;
                //This is the key call that will startup all services defined in the repository.  In this case, just the config and node services since the Simple Web app does not host any custom services.
                startupList = new List<ServiceHostInfo>(new[] { 
                    new ServiceHostInfo(false, null, new object[] { new Trade.BusinessServiceImplementation.ErrorBehaviorAttribute() }, new TradeServiceBSL()),
                    new ServiceHostInfo(false, null, new object[] { new Trade.BusinessServiceImplementation.ErrorBehaviorAttribute() }, new TradeServiceBSLRest()),
                    new ServiceHostInfo(false, null, new object[] { new Trade.BusinessServiceImplementation.ErrorBehaviorAttribute() }, new TradeServiceBSLResponse()) 
                });

                ServiceConfigHelper.MasterServiceWebHost MasterHost = new ServiceConfigHelper.MasterServiceWebHost(new Settings(), new ConfigurationService(), new NodeCommunication(), null, this, startupList, null, new object[] { typeof(IOrderProcessor), typeof(IQuoteService) }, null);
                setOrderMode();
                setQuoteMode();
            }
        }


        /// <summary>
        /// Constructor called when in self-host mode, since this logic is also
        /// used in IIS-host mode.  
        /// </summary>
        /// <param name="selfhost">True if in self-host mode.</param>
        public ConfigurationActions(bool selfhost)
        {
           
        }

        /// <summary>
        /// For BSL, this call initializes the connection string to the StockTraderDB, and the order mode as set
        /// in the repository.
        /// </summary>
        public void initBusinessService(object settingsInstance)
        {
            buildConnString();
            Settings.setTxModel();
            
            if (Settings.ORDER_PROCESSING_MODE == StockTraderUtility.OPS_INPROCESS)
            {
                try
                {
                    InitConfigInProcessOrderService.initConfigOrderProcessService(ConfigUtility.masterServiceWebHostSyncObject, true);
                }
                catch
                {
                    ConfigUtility.writeConsoleMessage("\nERROR INITIALIZING ORDER PROCESS SERVICE FROM REPOSITORY!  PLEASE EXAMINE EXCEPTION AND EXIT!", EventLogEntryType.Error, true, settingsInstance);
                    return;
                }
            }
        }

        /// <summary>
        /// This ensures we call required init procedure for BSL before opening a service host for business.  You can
        /// override this method in your own implementation for similar purposes, including making any manual code 
        /// adjustments to the ServiceHost description itself, if you want to.
        /// </summary>
        /// <param name="serviceHost">The created service host, all settings/endpoints already applied from repository.</param>
        /// <param name="VHostName">The name of the VHost that ties this to information in the repository.</param>
        /// <param name="settingsInstance">Instance of this host's Settings class.</param>
        /// <returns>Created ServiceHost, ready for Open()</returns>
        public override ServiceHost afterCreateServiceHostFromRepositoryBeforeOpen(ServiceHost serviceHost, string VHostName, object settingsInstance)
        {
            initBusinessService(settingsInstance);
            enableReceiveContextOnOperations(serviceHost, typeof(IOrderProcessorResponse), "OrderProcessed");
            return serviceHost;
        }

        /// <summary>
        /// Overrides the base method, performs app-specific logic on specific app-specific setting changes.  For BSL,
        /// there are few app specific routines we need to run when OPS-specific app settings are changed in the repository.
        /// MAKE SURE TO CALL THE BASE checkChangedSetting method FIRST!
        /// </summary>
        /// <param name="settingsInstance">Instance of this host's Settings class.</param>
        /// <param name="updatedConfigurationKey">The key that has been updated.</param>
        /// <param name="block">True to block on any multi-threaded calls, typically the node receiving via
        /// Config Service endpoint blocks, nodes receiving via Node Service do not, if any threads are spun up.</param>
        /// <returns>bool as success code.</returns>
        public override bool checkChangedSetting(object settingsInstance, ConfigurationKeyValues updatedConfigurationKey, bool block, ServiceUsers csUser)
        {
            bool success = false;
            success = base.checkChangedSetting(settingsInstance, updatedConfigurationKey, block, csUser); 
            if (success)
            {
                switch (updatedConfigurationKey.ConfigurationKeyFieldName)
                {

                    case "ORDER_PROCESSING_MODE":
                        {
                            setOrderMode();
                            ConfigUtility.invokeGenericConsoleMethod("PostInitProcedure", new object[] { Settings.ORDER_PROCESSING_MODE });
                            ConfigurationKeyValues orderServiceKey = new ConfigurationKeyValues();
                            switch (Settings.ORDER_PROCESSING_MODE)
                            {
                                case StockTraderUtility.OPS_INPROCESS:
                                    {
                                        InitConfigInProcessOrderService.initConfigOrderProcessService(ConfigUtility.masterServiceWebHostSyncObject, true);
                                        break;
                                    }
                            }

                            success = true;
                            break;
                        }

                    case "DAL":
                        {
                            
                            if (block)
                            {
                                ConfigurationKeyValues orderServiceKey = new ConfigurationKeyValues();
                                orderServiceKey.ConfigurationKeyFieldName = "DAL";
                                orderServiceKey.ConfigurationKeyValue = updatedConfigurationKey.ConfigurationKeyValue;
                                orderServiceKey.OriginatingConfigServiceName = "Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService";
                                refreshQSConfig(orderServiceKey, new Trade.BusinessServiceHostConfigurationImplementation.ConfigurationService(), block, csUser);
                            }
                            buildConnString();
                            break;
                        }

                        
                    case "DBServer":
                        {
                            buildConnString();
                            break;
                        }

                    case "Database":
                        {
                            buildConnString();
                            break;
                        }
                        

                    case "UserID":
                        {
                            buildConnString();
                            break;
                        }

                    case "Password":
                        {
                            buildConnString();
                            break;
                        }

                    case "MinDBConnections":
                        {
                            buildConnString();
                            break;
                        }

                    case "MaxDBConnections":
                        {
                            buildConnString();
                            break;
                        }

                    case "ENABLE_GLOBAL_SYSTEM_DOT_TRANSACTIONS_CONFIGSTRING":
                        {
                            Settings.setTxModel();
                            break;
                        }

                    case "QUOTE_MODE":
                        {
                            setQuoteMode();
                            ConfigUtility.invokeGenericConsoleMethod("PostInitProcedure", new object[] { Settings.QUOTE_MODE });
                            ConfigurationKeyValues quoteServiceKey = new ConfigurationKeyValues();
                            switch (Settings.QUOTE_MODE)
                            {
                                case StockTraderUtility.QS_INPROCESS:
                                    {
                                        InitConfigInProcessOrderService.initConfigOrderProcessService(ConfigUtility.masterServiceWebHostSyncObject, true);
                                        break;
                                    }
                                default:
                                    {
                                        if (block)
                                        {
                                            quoteServiceKey.ConfigurationKeyFieldName = "REFRESHCONFIG";
                                            quoteServiceKey.ConfigurationKeyValue = "RefreshConfig";
                                            quoteServiceKey.OriginatingConfigServiceName = "Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService";
                                            refreshQSConfig(quoteServiceKey, new ConfigurationService(), block, csUser);
                                        }
                                        break;
                                    }
                            }
                        }

                        break;

                   
                        
                    default:
                        {
                            success = true;
                            break;
                        }
                }
            }
            return success;
        }



        private static void setOrderMode()
        {
            Settings.OPS_CLIENT_ACTIVE_SERVICE_HOST_IDENTIFIER = StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE;
            Settings.QS_CLIENT_ACTIVE_SERVICE_HOST_IDENTIFIER = StockTraderUtility.QUOTE_SERVICE_AZURE;
        }

        private static void setQuoteMode()
        {
            Settings.QS_CLIENT_ACTIVE_SERVICE_HOST_IDENTIFIER = StockTraderUtility.QUOTE_SERVICE_AZURE;
            Settings.OPS_CLIENT_ACTIVE_SERVICE_HOST_IDENTIFIER = StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE;
        }

        /// <summary>
        /// Refreshes the Config of the QS when running in-process with BSL.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="configService"></param>
        /// <param name="csUser"></param>
        /// <returns>Int success code.</returns>
        public int refreshQSConfig(ConfigurationKeyValues key, ConfigurationService configService, bool notifyNodes, ServiceUsers csUser)
        {
            int returnCode = ConfigUtility.CLUSTER_UPDATE_FULL_SUCCESS;
            string value = key.ConfigurationKeyValue;
            string targetHostID = StockTraderUtility.QUOTE_SERVICE_AZURE;
            List<TraverseNode> traversePath = configService.getTraversePath(null, targetHostID, StockTraderUtility.QUOTE_SERVICE_CONFIG, csUser);
            key = configService.getServiceConfigurationKey(targetHostID, StockTraderUtility.QUOTE_SERVICE_CONFIG, key.ConfigurationKeyFieldName, traversePath, csUser);
            if (key != null)
            {
                key.ConfigurationKeyValue = value;
                traversePath = configService.getTraversePath(null, targetHostID, StockTraderUtility.QUOTE_SERVICE_CONFIG, csUser);
                if (traversePath != null || Trade.BusinessServiceConfigurationSettings.Settings.QUOTE_MODE.Equals(StockTraderUtility.QS_INPROCESS))
                    returnCode = configService.receiveConfigurationKey(targetHostID, StockTraderUtility.QUOTE_SERVICE_CONFIG, key, key, notifyNodes, ConfigUtility.UPDATE_KEY_VALUE, traversePath, csUser);
            }
            return returnCode;
        }

       

        /// <summary>
        /// This method builds a connection string based on DAL setting and settings for the database name, location, uid and password.
        /// Called on host initialization and also when the DAL or DB connection parameters are changed via ConfigWeb.
        /// </summary>
        private void buildConnString()
        {
            string conn_string = null;
            switch (Settings.DAL)
            {

                case StockTraderUtility.DAL_SQLAZURE:
                case StockTraderUtility.DAL_SQLAZUREFEDERATIONS:
                    {
                        if (!Settings.DBServer.Trim().ToLower().StartsWith("tcp:") && !Settings.DBServer.ToLower().Contains("sqlexpress"))
                            Settings.DBServer = "tcp:" + Settings.DBServer;
                        if (!Settings.DBServer.Trim().EndsWith(",1433") && !Settings.DBServer.ToLower().Contains("sqlexpress"))
                            Settings.DBServer = Settings.DBServer + ",1433";
                        conn_string = "server=" + Settings.DBServer + ";database=" + Settings.Database + ";user id=" + Settings.UserID + ";password=" + Settings.Password + ";min pool size=" + Settings.MinDBConnections + ";max pool size=" + Settings.MaxDBConnections;
                        if (Settings.DBServer.Trim().ToLower().Contains("database.windows.net") || Settings.DBServer.Trim().ToLower().Contains("sqlazurelabs"))
                            conn_string = conn_string + ";Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;Connect Timeout=60";
                        Settings.TRADEDB_SQL_CONN_STRING = conn_string;
                        try
                        {
                            SqlConnection.ClearAllPools();
                        }
                        catch { }
                        break;
                    }
            }

            Settings settingsInstance = new Settings();
            ServiceConfigHelper configHelper = new ServiceConfigHelper(settingsInstance);
            configHelper.InitConnectedDatabaseSettings(settingsInstance);
        }

        /// <summary>
        /// This is where you could implemement any additional validation logic for any config key.  For BSL, there are a few
        /// app-specific settings we need to validate.  If a setting update from ConfigWeb fails validation, the update to
        /// the repository (and other nodes) will be rejected automatically.  MAKE SURE TO CALL THE BASE VALIDATION FIRST.
        /// </summary>
        /// <param name="settingsInstance">Instance of this host's Settings class.</param>
        /// <param name="configurationKey">Key to be validated.</param>
        /// <returns>Bool return code, true if passes validation.</returns>
        public override bool validateConfigurationKey(object settingsInstance, ConfigurationKeyValues configurationKey)
        {
            //call base method first!
            bool success = base.validateConfigurationKey(settingsInstance, configurationKey);
            if (success)
            {
                switch (configurationKey.ConfigurationKeyFieldName)
                {
                    case "ORDER_PROCESSING_MODE":
                        {
                            success = configurationKey.ConfigurationKeyValue == StockTraderUtility.OPS_AZURE_HTTP_TMSEC_CLIENTCERT ||
                               configurationKey.ConfigurationKeyValue == StockTraderUtility.OPS_AZURE_HTTP_TMSEC_USERNAME ||
                               configurationKey.ConfigurationKeyValue == StockTraderUtility.OPS_AZURE_TCP_TMSEC_CLIENTCERT ||
                               configurationKey.ConfigurationKeyValue == StockTraderUtility.OPS_AZURE_TCP_TMSEC_USERNAME ||
                               configurationKey.ConfigurationKeyValue == StockTraderUtility.OPS_INPROCESS ||
                               configurationKey.ConfigurationKeyValue == StockTraderUtility.OPS_AZURE_SB;
                            break;
                        }
                    case "QUOTE_MODE":
                        {
                            success = configurationKey.ConfigurationKeyValue == StockTraderUtility.QS_AZURE_HTTP_TMSEC_CLIENTCERT ||
                                configurationKey.ConfigurationKeyValue == StockTraderUtility.QS_AZURE_HTTP_TMSEC_USERNAME ||
                                configurationKey.ConfigurationKeyValue == StockTraderUtility.QS_AZURE_TCP_TMSEC_CLIENTCERT ||
                                configurationKey.ConfigurationKeyValue == StockTraderUtility.QS_AZURE_TCP_TMSEC_USERNAME ||
                                configurationKey.ConfigurationKeyValue == StockTraderUtility.QS_INPROCESS;
                            break;
                        }
                }
            }
            return success;
        }
    }
}                          
