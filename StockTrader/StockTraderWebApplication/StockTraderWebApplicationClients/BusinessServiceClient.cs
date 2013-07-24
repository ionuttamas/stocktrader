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
using System.ServiceModel.Channels;
using ConfigService.LoadBalancingClient;
using Trade.BusinessServiceContract;
using Trade.BusinessServiceDataContract;
using Trade.StockTraderWebApplicationSettings;


namespace Trade.StockTraderWebApplicationServiceClient
{
    /// <summary>
    /// This is the WCF client class for the remote Business Services, connecting to any Web Service platform
    /// via WCF (.NET, J2EE etc.) that implements ITradeServices. This class implements channel initialization and
    /// load balancing/failover logic across cached channel instances specific to the Configuration Management/Node services
    /// implemented in StockTrader via the LoadBalancingClient.Client class, now re-used across all clients 
    /// implementing the configuration service. 
    /// </summary>
    [System.Runtime.Serialization.KnownType(typeof(AccountDataModel))]
    public class BusinessServiceClient : ITradeServices
    {
        public Client bslclient;

        public BusinessServiceClient(object settingsInstance)
        {
            bslclient = new Client(Settings.ACCESS_MODE, settingsInstance);
        }

        public ITradeServices Channel
        {
            get
            {
                return (ITradeServices)bslclient.Channel;
            }
            set
            {
                bslclient.Channel = (IChannel)value;
            }
        }

        public void emptyMethodAction()
        {
            try
            {
                this.Channel.emptyMethodAction();
            }
            catch 
            {
                this.Channel = null;
                throw;                       
            }
        }

        /// <summary>
        /// Logs user in/authenticates against StockTrader database.
        /// </summary>
        /// <param name="userid">User id to authenticate.</param>
        /// <param name="password">Password for authentication</param>
        public AccountDataModel login(string userID, string password)
        {
            try
            {
                return this.Channel.login(userID, password);
            }
            catch 
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets recent orders for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<OrderDataModel> getOrders(string userID)
        {
            try
            {
                return this.Channel.getOrders(userID);
            }
            catch 
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets account data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public AccountDataModel getAccountData(string userID)
        {
            try
            {
                return this.Channel.getAccountData(userID);
            }
            catch 
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets account profile data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public AccountProfileDataModel getAccountProfileData(string userID)
        {
            try
            {
                return this.Channel.getAccountProfileData(userID);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Updates account profile data for a user. 
        /// </summary>
        /// <param name="profileData">Profile data model class with updated info.</param>
        public AccountProfileDataModel updateAccountProfile(AccountProfileDataModel profileData)
        {
            try
            {
                return this.Channel.updateAccountProfile(profileData);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Logs a user out--updates logout count.
        /// </summary>
        /// <param name="userID">User id to logout.</param>
        public void logout(string userID)
        {
            try
            {
                this.Channel.logout(userID);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
            return;
        }

        /// <summary>
        /// Performs a stock buy operation.
        /// </summary>
        /// <param name="userID">User id to create/submit order for.</param>
        /// <param name="symbol">Stock symbol to buy.</param>
        /// <param name="quantity">Shares to buy</param>
        public OrderDataModel buy(string userID, string symbol, double quantity, int orderProcessingMode)
        {
            try
            {
                return this.Channel.buy(userID, symbol, quantity, orderProcessingMode);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Performs a holding sell operation.
        /// </summary>
        /// <param name="userID">User id to create/submit order for.</param>
        /// <param name="holdingID">Holding id to sell off.</param>
        /// <param name="quantity">Shares to sell.</param>
        public OrderDataModel sell(string userID, string holdingID, int orderProcessingMode)
        {
            try
            {
                return this.Channel.sell(userID, holdingID, orderProcessingMode);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets holding data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<HoldingDataModel> getHoldings(string userID)
        {
            try
            {
                return this.Channel.getHoldings(userID);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Registers/adds new user to database.
        /// </summary>
        /// <param name="userID">User id for account creation/login purposes as specified by user.</param>
        /// <param name="password">Password as specified by user.</param>
        /// <param name="fullname">Name as specified by user.</param>
        /// <param name="address">Address as specified by user.</param>
        /// <param name="email">Email as specified by user.</param>
        /// <param name="creditcard">Credit card number as specified by user.</param>
        /// <param name="openBalance">Open balance as specified by user. </param>
        public AccountDataModel register(string userID, string password, string fullname, string address, string email, string creditcard, decimal openBalance)
        {
            try
            {
                return this.Channel.register(userID, password, fullname, address, email, creditcard, openBalance);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Registers/adds new user device to database.
        /// </summary>
        /// <param name="userID">Existing user id.</param>
        /// <param name="deviceID">Unique device id.</param>
        /// <returns>Device logon details.</returns>
        public DeviceDataModel registerDevice(string userID, string deviceID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets any closed orders for a user--orders that have been processed.  Also updates status to complete.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<OrderDataModel> getClosedOrders(string userID)
        {
            try
            {
                return this.Channel.getClosedOrders(userID);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets the current market summary.  This results in an expensive DB query in the DAL; hence look to cache data returned for 60 second or so.
        /// </summary>
        public MarketSummaryDataModelWS getMarketSummary()
        {
            try
            {
                return this.Channel.getMarketSummary();
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets a single quote based on symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get data for.</param>
        public QuoteDataModel getQuote(string symbol)
        {
            try
            {
                return this.Channel.getQuote(symbol);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets quotes based on symbols string.
        /// </summary>
        /// <param name="symbol">Symbol to get data for.</param>
        public List<QuoteDataModel> getQuotes(string symbols)
        {
            try
            {
                return this.Channel.getQuotes(symbols);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }


        

        /// <summary>
        /// Gets a holding for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        /// <param name="holdingid">Holding id to retrieve data for.</param>
        public HoldingDataModel getHolding(string userID, string holdingID)
        {
            try
            {
                return this.Channel.getHolding(userID, holdingID);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets specific top n orders for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<OrderDataModel> getTopOrders(string userID)
        {
            try
            {
                return this.Channel.getTopOrders(userID);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Allows user to sell part of a holding vs. all.  Not implemented by Trade 6.1 on WebSphere.
        /// </summary>
        /// <param name="userID">User id to submit sell for.</param>
        /// <param name="holdingID">Holding id to sell.</param>
        /// <param name="quantity">Number of shares to sell.</param>
        public OrderDataModel sellEnhanced(string userID, string holdingID, double quantity)
        {
            try
            {
                return this.Channel.sellEnhanced(userID, holdingID, quantity);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Simple online check method.
        /// </summary>
        public void isOnline()
        {
            try
            {
                this.Channel.isOnline();
                return;
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }
    }
}
