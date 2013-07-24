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
// The OrderProcessor ConfigurationActions class, defining actions that get executed on Configuration Updates.
//======================================================================================================


using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Diagnostics;
using ConfigService.ServiceConfigurationBase;
using ConfigService.ServiceConfiguration.DataContract;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceNodeCommunication.DataContract;
using ConfigService.ServiceConfigurationUtility;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorServiceConfigurationSettings;
using Trade.OrderProcessorImplementation;
using Trade.Utility;

namespace Trade.OrderProcessorHostConfigurationImplementation
{
    /// <summary>
    /// This class is a core part of an implementation of the configuration management service/system.  
    /// This class should inherit from the base implementation ConfigurationActionsBase. The class,
    /// including methods defined in the base, performs configuration operations (actions) required to
    /// maintain the live running configuration system.
    /// </summary>
    public class ConfigurationActions : ConfigurationActionsBase  //Please inherit from base class!
    {

        public ConfigurationActions()
        {
            
        }

        /// <summary>
        /// For OPS, this call initializes the connection string to the StockTraderDB, and the order mode as set
        /// in the repository.
        /// </summary>
        public void initOrderForwardMode()
        {
            buildConnString();
        }

        /// <summary>
        /// This ensures we call required init procedure for OPS before opening a service host for business.  You can
        /// override this method in your own implementation for similar purposes, including making any manual code 
        /// adjustments to the ServiceHost description itself, if you want to.
        /// </summary>
        /// <param name="serviceHost">The created service host, all settings/endpoints already applied from repository.</param>
        /// <param name="hostName">The name of the VHost that ties this to information in the repository.</param>
        /// <param name="settingsInstance">Instance of this host's Settings class.</param>
        /// <returns>Created ServiceHost, ready for Open()</returns>
        public override ServiceHost afterCreateServiceHostFromRepositoryBeforeOpen(ServiceHost serviceHost, string hostName, object settingsInstance)
        {
            initOrderForwardMode();
            enableReceiveContextOnOperations(serviceHost, typeof(IOrderProcessor), settingsInstance, "SubmitOrder");
            return serviceHost;
        }

        /// <summary>
        /// This is where you could implemement any additional validation logic for any config key.  For OPS, there are a few
        /// app-specific settings we need to validate.  If a setting update from ConfigWeb fails validation, the update to
        /// the repository (and other nodes) will be rejected automatically.  MAKE SURE TO CALL THE BASE VALIDATION FIRST.
        /// </summary>
        /// <param name="settingsInstance">Instance of this host's Settings class.</param>
        /// <param name="configurationKey">Key to be validated.</param>
        /// <returns>Bool return code, true if passes validation.</returns>
        public override bool validateConfigurationKey(object settingsInstance, ConfigurationKeyValues configurationKey)
        {
            //First call base validate, this validates inherited settings, and integral type validaiton is also performed.
            bool success = base.validateConfigurationKey(settingsInstance, configurationKey);
            if (success)
            {
                switch (configurationKey.ConfigurationKeyFieldName)
                {
                    case "ORDER_RESPONSE_MODE":
                        success = configurationKey.ConfigurationKeyValue == StockTraderUtility.ORS_AZURE_HTTP_TMSEC_CLIENTCERT ||
                            configurationKey.ConfigurationKeyValue == StockTraderUtility.ORS_AZURE_HTTP_TMSEC_USERNAME ||
                            configurationKey.ConfigurationKeyValue == StockTraderUtility.ORS_AZURE_SB ||
                            configurationKey.ConfigurationKeyValue == StockTraderUtility.ORS_INPROCESS;
                        break;
                }
            }
            return success;
        }

        /// <summary>
        /// Overrides the base method, performs app-specific logic on specific app-specific setting changes.  For OPS,
        /// there are few app specific routines we need to run when OPS-specific app settings are changed in the repository.
        /// MAKE SURE TO CALL THE BASE checkChangedSetting method FIRST!
        /// </summary>
        /// <param name="settingsInstance">Instance of this host's Settings class.</param>
        /// <param name="updatedConfigurationKey">The key that has been updated.</param>
        /// <param name="block">True to block on any multi-threaded calls, typically the node receiving via
        /// Config Service endpoint blocks, nodes receiving via Node Service do not, if any threads are spun up.</param>
        /// <param name="csUser">The user making the requested configuration update.</param>
        /// <returns>bool as success code.</returns>
        public override bool checkChangedSetting(object settingsInstance, ConfigurationKeyValues updatedConfigurationKey, bool block, ServiceUsers csUser)
        {
            bool success = false;
            //First call the base method, this performs actions for required inherited global settings.
            success = base.checkChangedSetting(settingsInstance, updatedConfigurationKey, block, csUser);
            if (success)
            {
                switch (updatedConfigurationKey.ConfigurationKeyFieldName)
                {
                    
                    case "DAL":
                        {
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

                  
                    default:
                        {
                            success = true;
                            break;
                        }
                }
            }
            return success;
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
    }
}
