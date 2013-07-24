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

namespace Trade.Utility
{
    /// <summary>
    /// Utility class with constants used in StockTrader app/services. 
    /// </summary>
    public static class StockTraderUtility
    {
        //Exception Messages and Strings
        public static readonly string EXCEPTION_DOTNET_DUPLICATE_PRIMARY_KEY = "Violation of PRIMARY KEY";
        public static readonly string EXCEPTION_WEBSPHERE_DUPLICATE_PRIMARY_KEY = "org.omg.CORBA.portable.UnknownException";
        public static readonly string EXCEPTION_MESSAGE_INVALID_ORDERMODE_CONFIG = "This 'OrderMode' setting is not a valid setting (settings are case-sensitive). Please fix in: ";
        public static readonly string EXCEPTION_MESSAGE_INVALID_TXMMODEL_CONFIG = "This 'Use System.Transactions Globally' setting is not a valid setting.  Valid settings are 'true' or 'false'. Please fix in: ";
        public static readonly string EXCEPTION_MESSAGE_VALID_ORDERMODEVALUES = "Valid values are: 'Sync_InProcess', 'ASync_Msmq', 'ASync_Msmq_Volatile', 'ASync_Tcp', 'ASync_Http'.";
        public static readonly string EXCEPTION_MESSAGE_INVALID_HOLDING_BAD_QUOTE = "Holding with non-valid Quote Symbol: holdingID=";
        public static readonly string EXCEPTION_MESSAGE_INVALID_HOLDING_NOT_FOUND = "Holding not found!";
        public static readonly string EXCEPTION_MESSAGE_INVALID_HOLDING_ZERO_BASIS = "Holding with zero basis!: holdingID=";
        public static readonly string EXCEPTION_MESSAGE_NULL_DATE = "Data is Null. This method or property cannot be called on Null values.";
        public static readonly string EXCEPTION_MESSAGE_BAD_ORDER_PARMS = "Your order was not placed becuase the requested quantity was not valid.";
        public static readonly string EXCEPTION_MESSAGE_BAD_ORDER_RETURN = "We are sorry but your order could not be placed. Please try again later.";
        public static readonly string EXCEPTION_MESSAGE_ACID_REGISTRATION = "ACID TEST ON INSERT USERID 'ACID': PLANNED EXCEPTION THROWN!";
        public static readonly string EXCEPTION_MESSAGE_ACID_BUY = "PLANNED ACID TEST: SYMBOL 'ACIDBUY' PLANNED EXCEPTION THROWN!";
        public static readonly string EXCEPTION_MESSAGE_ACID_SELL = "PLANNED ACID TEST: SYMBOL 'ACIDSELL' PLANNED EXCEPTION THROWN!";
        public static readonly string EXCEPTION_MESSAGE_BADORDERTYPE = "Sorry, this order type is not allowed.";
        public static readonly string EXCEPTION_MESSAGE_INVALID_ORDERFORWARDBEHAVIOR_CONFIG = "This 'Order Processing Behavior' setting is not a valid setting (settings are case-sensitive). Please fix in: ";
        public static readonly string EXCEPTION_MESSAGE_INVALID_ORDERFORWARDMODE_CONFIG = "This 'OrderForwardMode' setting is not a valid setting (settings are case-sensitive). Please fix in: ";
        public static readonly string EXCEPTION_MESSAGE_VALID_ORDERFORWARDMODEVALUES = "Valid values are: 'ASync_Msmq', 'ASync_Msmq_Volatile', 'ASync_Tcp', 'ASync_Http'.";
        public static readonly string EXCEPTION_MESSAGE_VALID_ORDERFORWARDBEHAVIORVALUES = "Valid values are: 'Forward' and 'Standard'.";
        public static readonly string EXCEPTION_WEBSPHERE_USERID_NOTFOUND = "javax.ejb.ObjectNotFoundException";
        public static readonly string EXCEPTION_WEBSPHERE_INVALID_PASSWORD = "Incorrect password";
        public static readonly string EXCEPTION_MESSAGE_INVALID_ACCESSMODE_CONFIG = "This 'AccessMode' setting is not a valid setting (settings are case-sensitive): Please fix in: ";
        public static readonly string EXCEPTION_MESSAGE_VALID_ACCESSMODEVALUES = "Valid values are: 'InProcess' (for in-process activation), 'Http_WebService' (for SOA activation via WCF Self-Host/Http), 'Tcp_WebService' (for SOA activation via WCF Self-Host/Tcp), 'Asmx_WebService' (for SOA activation via ASMX) and 'WebSphere_WebService' (for SOA activation via WCF against IBM Trade 6.1 services).";
        public static readonly string EXCEPTION_MESSAGE_DUPLICATE_PRIMARY_KEY = "User ID Already Exists! Please Try a Different User ID.";
        public static readonly string EXCEPTION_MESSAGE_INVALID_LOGIN = "Error Logging In. Invalid Username or Password!";
        public static readonly string EXCEPTION_MESSAGE_INVALID_INPUT = "Invalid Input Was Detected.";
        public static readonly string EXCEPTION_MESSAGE_SQLSERVER_ADONET_PERMISSION_OR_NOT_FOUND_EXCEPTION = "The StockTrader Web Application is currently unable to perform this operation. The site may be under maintenance, so try again later.  A connection was requested by the application, and the StockTrader database is either not available or is not granting permission for this application-level connection.";
        public static readonly string EXCEPTION_MESSAGE_REMOTE_BSL_EXCEPTION = "The StockTrader Application is currently unable to perform this operation. The site may be under maintenance, so try again later.  The remote business service tier returned a fault or is currently unavailable.";
        public static readonly string EXCEPTION_MESSAGE_REMOTE_BSL_OFFLINE_EXCEPTION = "The StockTrader Application is currently unable to perform this operation. The site may be under maintenance, so try again later.  The remote business service tier is currently not available.";
        public static readonly string EXCEPTION_MESSAGE_REMOTE_OPS_EXCEPTION = "The StockTrader Application is currently unable to place an order, please try again later. The remote order processing service tier is currently unavailable to accept an order.";
        public const int SQL_ACCESS_DENIED_OR_DB_NOT_FOUND = -2146232060;
        //Define max lengths for input fields; 
        public static readonly int ADDRESS_MAX_LENGTH = 100;
        public static readonly int EMAIL_MAX_LENGTH = 100;
        public static readonly int CREDITCARD_MAX_LENGTH = 100;
        public static readonly int FULLNAME_MAX_LENGTH = 100;
        public static readonly int USERID_MAX_LENGTH = 50;
        public static readonly int PASSWORD_MAX_LENGTH = 50;
        public static readonly int OPENBALANCE_MAX_LENGTH = 20;
        public static readonly int QUOTESYMBOL_MAX_LENGTH = 10;
        public static readonly int SYMBOLSTRING_MAXLENGTH = 30;
        public static readonly int SALT_MAXLENGTH = 20;

        //Trade Business Logic constants
        public static readonly decimal BUY_FEE = 15.95m;
        public static readonly decimal SELL_FEE = 25.95m;
        public static readonly decimal PENNY_STOCK_P = .1m;
        public static readonly decimal STOCK_P_HIGH_BAR = 1000m;
        public static readonly decimal STOCK_P_HIGH_BAR_CRASH = .05m;
        public static readonly decimal JUNK_STOCK_MIRACLE_MULTIPLIER = 500m;
        public static readonly int STOCK_CHANGE_MAX_PERCENT = 5;


       

        //Regular Expression Validators
        public const string EXPRESSION_NAME_13 = "^[a-zA-Z''-'\\s]{0,13}$";
        public const string EXPRESSION_NAME_10 = "^[a-zA-Z''-'\\s]{0,13}$";
        public const string EXPRESSION_NAME_60 = "^[a-zA-Z''-'\\s]{1,60}$";
        public const string EXPRESSION_USERID = "^[-A-Za-z0-9\\s]{3,50}$";
        public const string EXPRESSION_QUOTES = "^[A-Za-z0-9'':'';'','\\s]{3,50}$";
        public const string EXPRESSION_QUOTE_ID = "^[A-Za-z0-9'':'\\s]{3,10}$";
        public const string EXPRESSION_HOLDINGID = "^(\\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\}{0,1})$";

       
        //Note we only differentiate between generic Web Service and WebSphere WebService because we have
        //implemented some additional UI and supporting Business Service functionality in StockTrader over and above
        //What Trade 6.1 provides, and we need to detect this case in just a couple of places in the Web app so
        //we do not try to make calls to Trade 6.1 that have no implementation.  Again, the exact same WCF client is used
        //as for the above possible AccessMode settings from Web app to Business Services.
        

        //map user strings to ints for faster lookups. We want to catch invalid settings
        //so the user knows what tx model they are running within a service.
        public const string TRANSACTION_STRING_SYSTEMDOTTRANSACTION_TRANSACTION = "true";
        public const string TRANSACTION_STRING_ADONET_TRANSACTION = "false";
        public const int TRANSACTION_MODEL_SYSTEMDOTTRANSACTION_TRANSACTION = 1;
        public const int TRANSACTION_MODEL_ADONET_TRANSACTION = 0;

        //ACID Test Codes
        public static readonly string ACID_TEST_USER = "ACIDUSER";
        public static readonly string ACID_TEST_BUY = "ACIDBUY";
        public static readonly string ACID_TEST_SELL = "ACIDSELL";
        public const string REGISTER_USER = "Register";

       

        //Service Host Names.  These are good candidates to move into the various repositories to make them more dynamic
        //than defined in code as constants.  Perhaps for next release of StockTrader--easy to do.
        public const string ORDER_PROCESSOR_SERVICE_AZURE = "Azure StockTrader Order Processor Service";
        public const string ORDER_PROCESSOR_SERVICE_CONFIG = "Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService";
        public const string QUOTE_SERVICE_AZURE = "Azure StockTrader Quote Service";
        public const string QUOTE_SERVICE_CONFIG = "Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService";
        public const string WEB_APPLICATION_Azure = "Azure StockTrader Web Application";
        public const string WEBAPPLICATION_CONFIG = "Trade.StockTraderWebApplicationConfigurationImplementation.ConfigurationService";
        public const string BUSINESS_SERVICES_CONFIG = "Trade.BusinessServiceHostConfigurationImplementation.ConfigurationService";
        public const string AZURE_BUSINESS_SERVICES = "Azure StockTrader Business Services";
        
        //Endpoint - Connected Service Names -- the various modes/bindings we can connect from Web app to Business Services

        //First for Business Services
        public const string BSL_INPROCESS = "In-Process Activation";
        public const string BSL_AZURE_WSHTTP_TMSEC_CLIENTCERT = "Azure BSL wsHttp security = TransportWithMessageCredential: ClientCertificate";
        public const string BSL_AZURE_WSHTTP_TMSEC_USERNAME = "Azure BSL wsHttp security = TransportWithMessageCredential: ClientUserName";
        public const string BSL_AZURE_TCP_TMSEC_CLIENTCERT = "Azure BSL netTcp security = TransportWithMessageCredential: ClientCertificate";
        public const string BSL_AZURE_TCP_TMSEC_USERNAME = "Azure BSL netTcp security = TransportWithMessageCredential: ClientUserName";
      
        //Now Endpoint - Connected Service Names for Order Processor
        public const string OPS_INPROCESS = "In-Process Activation";
        public const string OPS_AZURE_TCP_TMSEC_CLIENTCERT = "Azure OPS netTcp security = TransportWithMessageCredential: ClientCertificate";
        public const string OPS_AZURE_TCP_TMSEC_USERNAME = "Azure OPS netTcp security = TransportWithMessageCredential: ClientUserName";
        public const string OPS_AZURE_HTTP_TMSEC_CLIENTCERT = "Azure OPS wsHttp security = TransportWithMessageCredential: ClientCertificate";
        public const string OPS_AZURE_HTTP_TMSEC_USERNAME = "Azure OPS wsHttp security = TransportWithMessageCredential: ClientUserName";
        public const string OPS_AZURE_SB = "Azure OPS netMessaging";

        //Now Endpoint - Connected Service Names for Order Processor Response
        public const string ORS_INPROCESS = "In-Process Activation";
        public const string ORS_AZURE_HTTP_TMSEC_CLIENTCERT = "Azure ORS wsHttp security = TransportWithMessageCredential: ClientCertificate";
        public const string ORS_AZURE_HTTP_TMSEC_USERNAME = "Azure ORS wsHttp security = TransportWithMessageCredential: ClientUserName";
        public const string ORS_AZURE_SB = "Azure ORS netMessaging";

        //Now Endpoint - Connected Service Names for Quote Service
        public const string QS_INPROCESS = "In-Process Activation";
        public const string QS_AZURE_TCP_TMSEC_CLIENTCERT = "Azure QS netTcp security = TransportWithMessageCredential: ClientCertificate";
        public const string QS_AZURE_TCP_TMSEC_USERNAME = "Azure QS netTcp security = TransportWithMessageCredential: ClientUserName";
        public const string QS_AZURE_HTTP_TMSEC_CLIENTCERT = "Azure QS wsHttp security = TransportWithMessageCredential: ClientCertificate";
        public const string QS_AZURE_HTTP_TMSEC_USERNAME = "Azure QS wsHttp security = TransportWithMessageCredential: ClientUserName";

        //Order Codes
        public const string ORDER_TYPE_BUY = "buy";
        public const string ORDER_TYPE_SELL = "sell";
        public const string ORDER_TYPE_SELL_ENHANCED = "sellEnhanced";
        public static readonly string ORDER_STATUS_OPEN = "open";
        public static readonly string ORDER_STATUS_CLOSED = "closed";
        public static readonly string ORDER_STATUS_COMPLETED = "completed";

        //OOB DAL options
        public const string DAL_SQLAZURE = "Trade.DALSQLAzure";
        public const string DAL_SQLAZUREFEDERATIONS = "Trade.DALSQLAzure.Federations";

        //Open retry
        public const int SQL_OPEN_RETRY = 4;

        //Used to cache a Random class used for calculating random price changes vs. creating new
        //Random class on each buy/sell;  
        private static Random rand = new Random(DateTime.Now.Millisecond);

        //Dermines how much a stock price will change after a trade.
        public static decimal getRandomPriceChangeFactor(decimal current_price)
        {
            if (current_price <= PENNY_STOCK_P)
                return JUNK_STOCK_MIRACLE_MULTIPLIER;
            else if (current_price >= STOCK_P_HIGH_BAR)
                return STOCK_P_HIGH_BAR_CRASH;
            decimal factor = 0m;
            int y = rand.Next(1, STOCK_CHANGE_MAX_PERCENT);
            int x = rand.Next();
            if (x % 2 != 0)
            {
                factor = 1 - ((decimal)y) / 100m;
            }
            else
                factor = 1 + ((decimal)y) / 100m;
            return factor;
        }

        //How much the brokerage is going to charge!
        public static decimal getOrderFee(string orderType)
        {
            if (orderType.ToLower().Equals(ORDER_TYPE_BUY) || orderType.ToLower().Equals(ORDER_TYPE_SELL))
                return BUY_FEE;
            else
                return SELL_FEE;
        }
    }
}
