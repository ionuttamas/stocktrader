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
// The Settings class for the Business Service Layer (BSL).  Please note well that we only override the
// inherited settings (with the new keyword) becuase of the special case StockTrader BSL allows for running
// BSL in-process with the StockTrader Composite Web Application, vs. remote calls (and running the OPS in-process
// with the BSL, optionally).  You will NOT need to  do this for your services--your Settings class will be 
// much simpler, simply using the *inherited* global settings, and only specifying app-specific settings here.  
//======================================================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Channels;
using ConfigService.RuntimeHostData;
using ConfigService.ServiceConfiguration.DataContract;
using ConfigService.ServiceConfigurationBase;
using ConfigService.ServiceConfigurationUtility;
using ConfigService.ServiceNodeCommunication.DataContract;
using Trade.Utility;

namespace Trade.BusinessServiceConfigurationSettings
{
    /// <summary>
    /// PLEASE READ: A "Normal" implementation of Configuration Service will not have to define ANY HOST LEVEL SETTINGS as 'new object' here.
    /// BUSINESS SERVICE LAYER (BSL) AND OTHER STOCKTRADER 5 COMPONENTS ARE SPECIAL in that they can collapse down to run within the calling service process.
    /// This is 'In-Process Activation' setting.  NORMAL services will not do this---there is no need.  We do so here only to be able to easily
    /// BENCHMARK the entire application running with and WITHOUT WCF remote invocation.  In other words--how much does it cost performance-wise
    /// to call remote services; vs. just running a monolithic ASP.NET Web app.  Of course remote invocation will be slower, but, that is not really the point.
    /// A remote service is autonomous; may be running in a different service domain, controlled by a different dev team or company.  So, you can ignore
    /// all the settings here defined as 'new'; you will not need them as they will be inherited automatically and not hidden for a typical service that is
    /// not running other 'service' in-process.  But, you may be interested in the benchmark report that can show throughput and scalability numbers for this
    /// app, on Azure and on premise, running just as an ASP.NET Web app with no WCF services involved.  That is why we allow this special in-process mode. Sidebar done.  
    /// 
    /// 
    /// Any app or service implementing the Configuration Service needs a custom Settings class that will contain it's
    /// app-specific config settings. These are loaded from the SQL repository on startup with current values as set for service.
    /// Your custom settings class should inherit from the ConfigurationSettingsBase class, which contains common
    /// settings used by all services implementing the config service, in addition to the app-specific settings defined here.
    /// </summary>
    public class Settings : ConfigurationSettingsBase   //Inherit but override from base implementation!!
    {

        //This will override the setting in the base class.  Point this at whatever you name your key to store repository
        //connection string in your exe.config (or Web.config) local config file.  Or, if you name this config file key
        //'CONFIGDB_SQL_CONN_STRING', then you do not need to define this variable again, the base class will pick up
        //automatically.
        new public static string CONFIGDB_SQL_CONN_STRING;
       
        //Only necessary to support in-process OrderMode=Sync_InProcess.
        public static string OPS_CONFIGDB_SQL_CONN_STRING;
        new public static bool LOG_TO_DATABASE = false;


        //Note here we are overriding the base settings class field only becuase Business Services can
        //run in the special mode with Order Processor in the same process (OrderMode = 'Sync_InProcess'); as well the Web app can
        //optionally be run with the Business Services in process ( AccessMode= 'InProcess').  Becuase of this, we need to ensure
        //these common variables are NOT shared across the Web app Settings class, the
        //Order Processor Settings class, and the Business Servcies Settings class.  Hence, we create
        //fields here with the *new* keyword, so they become unique static variables owned by this specific Settings
        //class (in this case Business Services). You would typically not need to do this.
        //These fields are initialized at runtime based on the repository settings and started services.
        new public static object thisService;
        new public static object thisServiceConfigActions;
        new public static List<ServiceUsers> thisServiceUsers;
        new public static List<ConnectedServices> connectedServices;
        new public static List<ConnectedConfigServices> connectedConfigServices;
        new public static Dictionary<string, object> connectedServiceTypes;
        new public static Dictionary<string, object> connectedServiceDefaultContexts;
        new public static Dictionary<string, string> connectedServiceOnlineMethods;
        new public static Dictionary<string, object[]> connectedServiceOnlineParms;
        new public static List<BindingInformation> bindingInformation;
        new public static List<ClientInformation> clientInformation;
        new public static Dictionary<string, Binding> bindingDictionary;
        new public static Dictionary<string, Binding> bindingMexDictionary;
        new public static List<HostListenEndPointInstance> connectionPointList;
        new public static List<MasterServiceHostInstance> serviceHosts;
        new public static List<ServiceHostInfo> startupServiceHosts;
        new public static EndPointBehaviors endpointBehaviors;
        new public static List<HostedServices> hostedServices;
        new public static Dictionary<int, ServiceHostInfo> serviceHostInfo;
        new public static Dictionary<int, string> hostedServiceOnlineMethods;
        new public static Dictionary<int, object[]> hostedServiceOnlineParms;
        new public static List<HostListenEndPointInstance> hostConfigListenEndpoints;
        new public static List<HostListenEndPointInstance> hostPrimaryListenEndpoints;
        new public static List<HostListenEndPointInstance> hostNodeListenEndpoints;
        new public static List<HostListenEndPointInstance> hostNodeDCListenEndpoints;
        new public static Dictionary<int, Uri> hostListenUris;
        new public static ConnectionStringSettings configDB;
        new public static List<ConnectedDBs> connectedDBs;
        new public static string connected_database1;
        new public static string connected_database2;
        new public static string connected_database3;
        new public static long invokeCount = 0L;
        new public static List<DistributedCaches> connectedCaches;
        new public static string connected_cache_cache1;
        new public static string connected_cache_servers_with_ports_cache1;
        new public static string connected_cache_authtoken_cache1;
        new public static string connected_cache_cache2;
        new public static string connected_cache_servers_with_ports_cache2;
        new public static string connected_cache_authtoken_cache2;
        new public static string connected_cache_cache3;
        new public static string connected_cache_servers_with_ports_cache3;
        new public static string connected_cache_authtoken_cache3;
        new public static int PERSIST_REQUEST_INTERVAL = 60000;
        new public static bool LOG_REQUESTS = true;

        //Begin Repository Settings ---------------------------------

        //These are your application-specific configuration settings for this app or service.
        //Each of these configuration settings has a corresponding row in the config configuration database
        //ConfigurationKeys table.  This table contains the values that will be set for each field defined
        //below.

        //If set to false via ConfigWeb, will turn off sharing of MetaData from configuration database between services.
        new public static bool AUTO_SUPPLY_CS_ID_PASSWORD = true;
        new public static bool SHARESERVICECONFIGURATIONDATA = true;

        //If set to false via ConfigWeb, will prevent a service host from querying connected services for their
        //config data.
        new public static bool CONFIGSHOWCONNECTEDSERVICES = true;
        new public static string SERVICEVERSION = "";
        new public static string SERVICEHOSTER = "";
        new public static string SERVICEPLATFORM = "";
        new public static string ADMINISTRATORCONTACT = "";

        //Reset on app startup from configuration database to Event Log Source name defined in configuration database for an app/service.
        new public static string EVENT_LOG = ConfigSettings.EVENT_LOG;

        //Turns on/off logging of informational message logging in consoles and Event Log.  Warnings and Errors will still be logged.
        new public static bool DETAILED_LOGGING = true;

        //Turns on/off logging of information for node notification service to consoles and the Event Log. Make sure off for production.
        new public static bool NODE_LOG_NOTIFICATIONS = true;

        //Can optionally adjust the number of WCF Service Channels per unique URI in load-balancing configuration.
        //Typically 1 channel per URI is plenty, even for very high-traffic, high performance services, given how
        //WCF manages these within the infrastructure.
        new public static int NUM_CHANNELS = 0;

        //Identifies a unique service host.
        new public static string HOST_NAME_IDENTIFIER = "";
        new public static string CONFIG_SERVICENAME_IDENTIFIER = "";
        new public static string VHOST_NAME_IDENTIFIER = "";

        //Used to force refresh of entire set of settings from configuration database.
        new public static string REFRESHCONFIG = "";

        //Number of consecutive bad requests to a specific URI before removing the URI from subsequent load-balanced requests.
        //Note the notification system works such that restarted hosts will auto-rejoin the load-balancing, and each
        //service client independently maintains its own list of load-balanced service nodes per service contract.
        new public static int BAD_REQUEST_LIMIT = 0;

        //The number of cycles through an entire load-balanced list of servers without getting a valid response before
        //returning an exception of No Hosts Online.
        new public static int RETRY_CYCLES = 0;

        //Equivalent to <System.Net> config setting, determines max number of HTTP connections from a service client to a
        //service host when communicating over http.  This is an important .NET tuning settings for Web service clients.  We
        //use store in the Service configuration database to make changing this easy.
        new public static int DEFAULT_CONNECTION_LIMIT = 0;

        //If you are hosting in a Windows Application (via the base Windows Host Console class), you can set this to true, to
        //supress all Windows Popups (on warning/error conditions) for a production environment.
        new public static bool SUPRESS_WIN_POPUPS = true;

        //This is a key setting as it determines over which endpoint 
        //(inclusive of transport/encoding/binding and hence security) nodes communicate with each other.
        //If select the specific Channel shape to use; you can support many channel shapes, but only one
        //will be active at a time based on this setting.
        new public static int NODE_ACTIVE_SERVICE_ID = 0;

        //This is a key setting as it determines over which endpoint 
        //(inclusive of transport/encoding/binding and hence security) nodes communicate with each other for distributed/replicated
        //cache operations.
        new public static int NODE_ACTIVE_DC_SERVICE_ID = 0;

        //This is the maximum amount of time in milliseconds any Master Controller thread in the Config Service
        //will wait for its children threads before returning.  Also is the maximum amount of time
        //any VHostController thread will wait for any of its children to respond. In all cases,
        //the first thread to get a response signals, releasing the Master Controller or VHost controller
        //at that moment, and the other children threads will simply exit on their own.  First responder wins.
        new public static int MAX_THREADWAIT_TIMEOUT = 0;

        //Inner thread wait timeout.
        new public static int MAX_INNER_THREADWAIT_TIMEOUT = 0;

        //URI thread wait timeout.
        new public static int MAX_URI_THREADWAIT_TIMEOUT = 0;

        //Used in online node notifications.  In general, do not adjust below 20000 ms (20 seconds).  Some node notification operations, such
        //as receieveService, may take this long to complete considering shutting down service host, re-initializing from configuration database, and re-opening
        //across all clustered nodes.
        new public static int MAX_NODE_NOTIFY_THREADWAIT_TIMEOUT = 0;

        //Used in online node checks during load balancing operations for primary services. Max amount of time in ms a node is given to
        //respond before client attempts to find a different node.  Important for scenarios where a server has been unexpectedly powered down,
        //or lost net connectivity, otherwise, such a failure would cause longer duration throughput drops. 
        new public static int MAX_PRIMARY_SERVICE_THREADWAIT_TIMEOUT = 0;

        //The wait period before nodes re-intialize node communication channels after a service definition change.  Make no shorter than 10000 ms
        //to ensure time for service hosts restart procedures across peer nodes.
        new public static int CHANGEDSERVICE_THREADWAIT_TIMEOUT = 0;

        new public static int NODE_ACTIVE_ID = 0;
        new public static int NODE_DC_ACTIVE_ID = 0;

        new public static bool ACCEPT_ALL_CERTIFICATES = false;

        // Settings not directly loaded from Service Repository, but set on startup based on repository 
        // and/or server-specific runtime information. 
        new public static List<int> NODE_IDS;
        new public static List<int> NODE_DC_IDS;
        //Unique per Node:
        new public static string NODE_ACTIVE_ADDRESS;
        new public static int TOPQUERY_ANALYSIS = 10;

        //Unless you specify an appropriate remote HOST NAME IDENTIFIER (or several, separate by ;), navigation to remote services via config web will be prohibited.
        new public static string ALLOWED_LINKED_SERVICES = "";

        //Millisecond timeout when checking Connection Point online status/ala ConfigWeb's ConnectionPoint.aspx page.
        //If a response is not received in the allotted time, a status of 'unknown' will be returned (grey server icon).
        new public static int CONNECTIONPOINT_CHECK_TIMEOUT = 0;
        new public static int DATABASE_CHECK_TIMEOUT = 0;

        //BEGIN BUSINESS SERVICE LAYER (BSL) REPOSITORY DEFINED SETTINGS.  ANY CONFIG KEY SETTING DEFINED IN DB REPOSITORY MUST HAVE A MATCHING STATIC VARIABLE HERE.
        //NOTE WELL:  THE VARIABLE NAME WILL MATCH THE REPOSITORY COLUMN SettingsClassFieldName IN THE TABLE CONFIGURATIONKEYS.  
        //            THE CONFIG MANAGEMENT SYSTEM AUTOMATICALLY COORDINATES/SYNCHRONIZES CONFIG SETTINGS ACROSS RUNNING
        //            HOST INSTANCES ON DIFFERENT SERVERS. THEY ARE SET VIA REFLECTION ON APP INITIALIZATION FROM THE SERVICE REPOSITORY,
        //            YET UPDATABLE ACROSS DISTRIBUTED HOST INSTANCES VIA THE CONFIG MANAGEMENT SYSTEM (CONFIGWEB).  YOU WILL USE CONFIGWEB TO ADD
        //            REPOSITORY CONFIGURATION KEYS TO MAP THE REPOSITORY SETTING TO A CUSTOM (APP-SPECIFIC) FIELD NAME IN THE SETTINGS CLASS, AS
        //            WELL AS UPDATE SETTINGS ON LIVE CLUSTERED SYSTEMS WITHOUT APP RESTARTS.

        public static string OPS_CLIENT_CONNECTED_SERVICE_CONTRACT;
        public static string ORDER_PROCESSING_MODE;
        public static string QUOTE_MODE; 
        public static string DBServer; 
        public static string Database; 
        public static string UserID; 
        public static string Password;
        public static string NEWS_MANAGEMENT_SECRET;
        public static string NEWS_TOPIC;
        public static string NEWS_NAMESPACE;
        public static int MinDBConnections; 
        public static int MaxDBConnections; 
        public static string TRADEDB_SQL_CONN_STRING; 
        public static string DAL; 
        public static string ENABLE_GLOBAL_SYSTEM_DOT_TRANSACTIONS_CONFIGSTRING; 
        public static int SYSTEMDOTTRANSACTION_TIMEOUT; 
        public static int MAX_QUERY_ORDERS; 
        public static int MAX_QUERY_TOP_ORDERS; 
        public static bool DISPLAY_WEBSERVICE_LOGINS; 
        public static int LOGIN_ITERATIONSTO_DISPLAY;
        public static bool USE_SALTEDHASH_PASSWORDS;
       
       
        
        //End Repository Settings -----------------------------------

        
        /// <summary>
        /// Local Settings Not From Config Database Repository.  Note that any of these could optionally be moved into the 
        /// config repository instead of being initialized in code.
        /// </summary>
        /// 
        public static string OPS_CLIENT_ACTIVE_SERVICE_HOST_IDENTIFIER = "";

        public static string QS_CLIENT_ACTIVE_SERVICE_HOST_IDENTIFIER = "";
        
        //set on startup based on config settings from repository
        public static int TRANSACTION_MODEL = -1;
        
        /// <summary>
        /// Sets the selected transaction model to use to an int constant. 
        /// </summary>
        public static void setTxModel()
        {
            switch (ENABLE_GLOBAL_SYSTEM_DOT_TRANSACTIONS_CONFIGSTRING)
            {
                case (StockTraderUtility.TRANSACTION_STRING_SYSTEMDOTTRANSACTION_TRANSACTION):
                    {
                        TRANSACTION_MODEL = StockTraderUtility.TRANSACTION_MODEL_SYSTEMDOTTRANSACTION_TRANSACTION;
                        break;
                    }
                case (StockTraderUtility.TRANSACTION_STRING_ADONET_TRANSACTION):
                    {
                        TRANSACTION_MODEL = StockTraderUtility.TRANSACTION_MODEL_ADONET_TRANSACTION;
                        break;
                    }
            }
            return; 
        }
    }
}