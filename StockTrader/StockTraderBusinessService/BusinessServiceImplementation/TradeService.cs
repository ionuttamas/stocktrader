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
//  TradeServiceFacadeWcf:  The WCF/.NET 3.5 Web Service facade to TradeService.cs. 
//======================================================================================================


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using ConfigService.ServiceConfigurationUtility;
using Trade.BusinessServiceConfigurationSettings;
using Trade.BusinessServiceContract;
using Trade.BusinessServiceDataContract;
using Trade.Utility;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// This is the service facade implementation for WCF-based Trade Business Services. It defines the business service layer operations
    /// that are implemented in the TradeEngine.cs implementation class.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class TradeServiceBSL : ITradeServices
    {
        static int loginCount;
        
        public TradeServiceBSL()
        {
            Interlocked.Increment(ref Settings.invokeCount);
        }

        //Used for online check in WCF proxy logic for clients to this service; employed in 
        //StockTrader load-balanced scenarios to ensure application-level failover of 
        //service-to-service remote calls to clusters running this service. 
        public void isOnline()
        {
            //We will not count this online check from Remote client as an activation, since its not a real operation.  Ultimately,
            //there is a TODO here, which is apply a WCF endpoint behavior, such that we count actual requests at the service op level.
            //Also note while this will be called from ConfigWeb for admin procedures (and discounted here); it will not be called
            //when using an external load balancer; or if the known node count=1.  There is no point in this check, if there is no
            //node with a different address to failover to.  So, for on-premise, this kicks in with 2 or more nodes, with the
            //benefit of service-op level failover.  With external load balancers, its up to them to decide at what level
            //they check online status.  It might be at the (virtual) machine level; or at the endpoint level.
            Interlocked.Decrement(ref Settings.invokeCount);
        }

        //Here only for some obscure Java interop with prior WebSphere versions.
        public void emptyMethodAction()
        {
        }

        /// <summary>
        /// Logins the specified userid.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public AccountDataModel login(string userid, string password)
        {
            loginCount++;
            if (Settings.DISPLAY_WEBSERVICE_LOGINS && (loginCount % Settings.LOGIN_ITERATIONSTO_DISPLAY == 0))
                ConfigUtility.writeConsoleMessage("Login request # " + loginCount.ToString() + " received. Login is for user id: " + userid + "\n",EventLogEntryType.Information,false, new Settings());
            return new TradeEngine().login(InputText(userid, StockTraderUtility.USERID_MAX_LENGTH), InputText(password, StockTraderUtility.PASSWORD_MAX_LENGTH));             
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual List<OrderDataModel> getOrders(string userID)
        {
            return new TradeEngine().getOrders(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Gets the account data.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual AccountDataModel getAccountData(string userID)
        {
            return new TradeEngine().getAccountData(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Gets the account profile data.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual AccountProfileDataModel getAccountProfileData(string userID)
        {
            return new TradeEngine().getAccountProfileData(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Updates the account profile.
        /// </summary>
        /// <param name="profileData">The profile data.</param>
        public virtual AccountProfileDataModel updateAccountProfile(AccountProfileDataModel profileData)
        {
            return new TradeEngine().updateAccountProfile(profileData);
        }

        /// <summary>
        /// Logouts the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual void logout(string userID)
        {
            new TradeEngine().logout(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Buys the specified symbol.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="orderProcessingMode">The order processing mode.</param>
        public virtual OrderDataModel buy(string userID, string symbol, double quantity, int orderProcessingMode)
        {
            //note orderProcessing mode param is not used by StockTrader; instead
            //app picks this up from the application configuration.
            if (quantity <= 0)
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_BAD_ORDER_PARMS);
            return new TradeEngine().buy(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH), InputText(symbol, StockTraderUtility.QUOTESYMBOL_MAX_LENGTH), quantity, orderProcessingMode);
        }

        /// <summary>
        /// Sells the specified holding.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        /// <param name="orderProcessingMode">The order processing mode.</param>
        public virtual OrderDataModel sell(string userID, string holdingID, int orderProcessingMode)
        {
            //note orderProcessing mode param is not used by StockTrader; instead
            //app picks this up from the application configuration.
            return new TradeEngine().sell(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH), holdingID, orderProcessingMode);
        }

        /// <summary>
        /// Gets the holdings for the user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual List<HoldingDataModel> getHoldings(string userID)
        {
            return new TradeEngine().getHoldings(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Registers the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        /// <param name="fullname">The fullname.</param>
        /// <param name="address">The address.</param>
        /// <param name="email">The email.</param>
        /// <param name="creditcard">The creditcard.</param>
        /// <param name="openBalance">The open balance.</param>
        public AccountDataModel register(string userID, string password, string fullname, string address, string email, string creditcard, decimal openBalance)
        {
            return new TradeEngine().register(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH), InputText(password, StockTraderUtility.PASSWORD_MAX_LENGTH), InputText(fullname, StockTraderUtility.FULLNAME_MAX_LENGTH), InputText(address, StockTraderUtility.ADDRESS_MAX_LENGTH), InputText(email, StockTraderUtility.EMAIL_MAX_LENGTH), InputText(creditcard, StockTraderUtility.CREDITCARD_MAX_LENGTH), openBalance);
        }

        /// <summary>
        /// Registers the device for the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="deviceID">The device ID.</param>
        public virtual DeviceDataModel registerDevice(string userID, string deviceID)
        {
            return new TradeEngine().registerDevice(userID, deviceID);
        }

        /// <summary>
        /// Gets the closed orders for the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual List<OrderDataModel> getClosedOrders(string userID)
        {
            return new TradeEngine().getClosedOrders(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Gets the market summary.
        /// </summary>
        public MarketSummaryDataModelWS getMarketSummary()
        {
            return new TradeEngine().getMarketSummary();
        }

        /// <summary>
        /// Gets the quote for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public QuoteDataModel getQuote(string symbol)
        {
            string _symbol = InputText(symbol, StockTraderUtility.QUOTESYMBOL_MAX_LENGTH);
            if (_symbol.Length == 0)
                return null;
            return new TradeEngine().getQuote(InputText(symbol, StockTraderUtility.QUOTESYMBOL_MAX_LENGTH));
        }

        /// <summary>
        /// Gets quotes for the given symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        public List<QuoteDataModel> getQuotes(string symbols)
        {
            string _symbols = InputText(symbols, 100);
            if (_symbols.Length == 0)
                return null;
            return new TradeEngine().getQuotes(_symbols);
        }

        /// <summary>
        /// Gets the holding specified by ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        public virtual HoldingDataModel getHolding(string userID, string holdingID)
        {
            return new TradeEngine().getHolding(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH), holdingID);
        }

        /// <summary>
        /// Gets the top *n* orders for a user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual List<OrderDataModel> getTopOrders(string userID)
        {
            return new TradeEngine().getTopOrders(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH));
        }

        /// <summary>
        /// Sells part or all of the specified holding.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        /// <param name="quantity">The quantity.</param>
        public virtual OrderDataModel sellEnhanced(string userID, string holdingID, double quantity)
        {
            if (quantity <= 0)
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_BAD_ORDER_PARMS);
            return new TradeEngine().sellEnhanced(InputText(userID, StockTraderUtility.USERID_MAX_LENGTH), holdingID, quantity);
        }

        /// <summary>
        /// Cleans up the input text.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="maxLength">Length of the max.</param>
        private static string InputText(string inputString, int maxLength)
        {
            // check incoming parameters for null or blank string
            if (!string.IsNullOrEmpty(inputString))
            {
                inputString = inputString.Trim();

                //chop the string incase the client-side max length
                //fields are bypassed to prevent buffer over-runs
                if (inputString.Length > maxLength)
                    inputString = inputString.Substring(0, maxLength);
            }

            return inputString;
        }
    }
}
