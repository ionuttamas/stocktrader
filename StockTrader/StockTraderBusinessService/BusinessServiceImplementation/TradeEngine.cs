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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using Trade.BusinessServiceConfigurationSettings;
using Trade.BusinessServiceDataContract;
using Trade.DALFactory;
using Trade.IDAL;
using Trade.OrderProcessorAsyncClient;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorImplementation;
using Trade.Utility;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// Base class for order processing operations.
    /// </summary>
    class TradeEngine
    {
        public void emptyMethodAction()
        {
        }
        
        public void isOnline()
        {
        }

        /// <summary>
        /// Logs user in/authenticates against StockTrader database.
        /// </summary>
        /// <param name="userid">User id to authenticate.</param>
        /// <param name="password">Password for authentication</param>
        public AccountDataModel login(string userid, string password)
        {
            //Create instance of a DAL, which could be designed for any type of DB backend.
            ICustomer dalCustomer = Customer.Create(Settings.DAL);

            //As feature of the StockTrader DAL, you will see dal.Open, dal.BeginTransaction, dal.CommitTransaction,
            //dal.AbortTransaction and dal.Close methods being invoked in the BSL. The pattern  within this BSL is:
            //a) Create an instance of the DAL; 
            //b) Open the DAL; 
            //c) Start a transaction only if necessary (more than one update/insert/delete involved);
            //d) You get to pick ADO.NET transaction or System.Transactions or ServicedComponent, it will work with
            //   all of the above; StockTrader lets you choose ADO.NET txs or System.Transactions via config.
            //e) Close the DAL. This releases the DAL's internal connection back to the connection pool.

            //The implementation "hides" the type of database being used from the BSL, so this BSL will work
            //with any type of database you create a DAL for, with no changes in the BSL whatsoever. 

            //System.Transactions and SQL Server 2005 and above and Oracle databases work together
            //with a new feature called "lightweight transactions"; which means you do not need to have the
            //same performance penalty you got with Serviced Components for always invoking the tx as a full
            //two-phase operation with DTC logging.  If operating against a single database to SQL Server or Oracle,
            //across one or more connections involved in a tx, System.Transactions will not promote to a DTC-coordinated tx; and hence will be much faster.
            //If there are mulitple databases or multiple resources (for example, MSMQ and a database) 
            //used with a System.Transaction tx, on the other hand, the tx will be automatically promoted to the required distributed tx, two-phase commit
            //with DTC logging required. Our StockTrader DAL is designed to:

            // 1.  Hide DB implementation from BSL so we maintain clean separation of BSL from DAL.
            // 2.  Let you freely call into the DAL from BSL methods as many times as you want *without*
            //     creating new separate DB connections
            // 3.  As a by-product, it also helps you use ADO.NET transactions without worrying about
            //     passing DB connections/transaction objects between tiers; maintaining cleaner separation
            //     of BSL from DAL.  If using ADO.NET txs; you can accomplish DB-implementation isolation also with
            //     the Provider Factories introduced with ADO.NET 2.0/.NET 2.0: see for details:

            //             http://msdn2.microsoft.com/en-us/library/ms379620(VS.80).aspx

            //Note Open() is not really necessary, since the DAL will open a new connection automatically 
            //if its internal connection is not already open.  It's also free to open up more connections, if desired.
            //We use Open() to stick with a consistent pattern in this application, since the Close() method IS
            //important.  Look for this pattern in all BSL methods below; with a transaction scope defined
            //only for operations that actually require a transaction per line (c) above.
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.login(userid, password, Settings.USE_SALTEDHASH_PASSWORDS);
            }
            finally
            {
                //Always close the DAL, this releases its primary DB connection.
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Logs a user out--updates logout count.
        /// </summary>
        /// <param name="userID">User id to logout.</param>
        public void logout(string userID)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                dalCustomer.logOutUser(userID);
            }
            finally
            {
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Validates the specified user/pass combination.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        public bool validate(string userID, string password)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.validate(userID, password, Settings.USE_SALTEDHASH_PASSWORDS);
            }
            finally
            {
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Gets account data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public AccountDataModel getAccountData(string userID)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.getCustomerByUserID(userID);
            }
            finally
            {
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Gets account profile data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public AccountProfileDataModel getAccountProfileData(string userID)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.getAccountProfileData(userID);
            }
            finally
            {
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Gets recent orders for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<OrderDataModel> getOrders(string userID)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.getOrders(userID, false, Settings.MAX_QUERY_TOP_ORDERS, Settings.MAX_QUERY_ORDERS);
            }
            finally
            {
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Gets specific top n orders for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<OrderDataModel> getTopOrders(string userID)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.getOrders(userID, true, Settings.MAX_QUERY_TOP_ORDERS, Settings.MAX_QUERY_ORDERS);
            }
            finally
            {
                dalCustomer.Close();
            }
        }

        /// <summary>
        /// Gets any closed orders for a user--orders that have been processed.  Also updates status to complete.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<OrderDataModel> getClosedOrders(string userID)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.getClosedOrders(userID);
            }
            finally
            {
                dalCustomer.Close();
            }

        }

        /// <summary>
        /// Performs a holding sell operation.
        /// Note orderProcessing mode param is not used by StockTrader; instead
        /// app picks this up from the application configuration.
        /// </summary>
        /// <param name="userID">User id to create/submit order for.</param>
        /// <param name="holdingID">Holding id to sell off.</param>
        /// <param name="orderProcessingMode">Not used, set to zero.</param>
        public OrderDataModel sell(string userID, string holdingID, int orderProcessingMode)
        {
            //In the case of running in 'Sync_InProcess' mode, then the PlaceOrder method 
            //will synchronously invoke the processOrder method as part of this service, and not make an 
            //additional remote service-to-sevice call via WCF. See ProcessOrder.cs.
            //note, this method always sells entire holding, quantity is not passed in.  This is default behavior of WebSphere Trade 6.1
            return placeOrder(StockTraderUtility.ORDER_TYPE_SELL, userID, holdingID, null, 0);
        }

        /// <summary>
        /// Allows user to sell part of a holding vs. all.  
        /// This is added functionality that .NET StockTrader implements to allow selling of partial 
        /// holdings, vs. liquidating the entire holding at once.  
        /// </summary>
        /// <param name="userID">User id to submit sell for.</param>
        /// <param name="holdingID">Holding id to sell.</param>
        /// <param name="quantity">Number of shares to sell.</param>
        public OrderDataModel sellEnhanced(string userID, string holdingID, double quantity)
        {
            return placeOrder(StockTraderUtility.ORDER_TYPE_SELL_ENHANCED, userID, holdingID, null, quantity);
        }

        /// <summary>
        /// Performs a stock buy operation.
        /// Note orderProcessing mode param is not used by StockTrader; instead
        /// app picks this up from the application configuration.  
        /// </summary>
        /// <param name="userID">User id to create/submit order for.</param>
        /// <param name="symbol">Stock symbol to buy.</param>
        /// <param name="quantity">Shares to buy.</param>
        ///<param name="orderProcessingMode">Not used.</param>
        public OrderDataModel buy(string userID, string symbol, double quantity, int orderProcessingMode)
        {
            return placeOrder(StockTraderUtility.ORDER_TYPE_BUY, userID, null, symbol, quantity);
        }

        /// <summary>
        /// Updates account profile data for a user. 
        /// </summary>
        /// <param name="profileData">Profile data model class with updated info.</param>
        public AccountProfileDataModel updateAccountProfile(AccountProfileDataModel profileData)
        {
            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            try
            {
                return dalCustomer.update(profileData, Settings.USE_SALTEDHASH_PASSWORDS);
            }
            finally
            {
                dalCustomer.Close();
            }
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
        /// <returns>Account details.</returns>
        public AccountDataModel register(string userID, string password, string fullname, string address, string email, string creditcard, decimal openBalance)
        {
            // In order to support basic auth, we need to prohibit certain characters
            if (userID == null || userID.Contains(":") || userID.Contains("\\"))
            {
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_INVALID_INPUT);
            }

            AccountDataModel newCustomer = null;

            //Switch is two let you configure which transaction model you want to benchmark/test.
            switch (Settings.TRANSACTION_MODEL)
            {
                case (StockTraderUtility.TRANSACTION_MODEL_SYSTEMDOTTRANSACTION_TRANSACTION):
                    {
                        CheckConnection();

                        ICustomer dalCustomer = Customer.Create(Settings.DAL);
                        dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);

                        try
                        {
                            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions()))
                            {
                                newCustomer = addNewRegisteredUser(dalCustomer, userID, password, fullname, address, email, creditcard, openBalance);
                                //Scope complete, commit work.
                                tx.Complete();
                            }
                        }
                        finally
                        {
                            dalCustomer.Close();
                        }

                        break;
                    }

                case (StockTraderUtility.TRANSACTION_MODEL_ADONET_TRANSACTION):
                    {
                        //ADO.NET TX case:  First you need to open the connecton.
                        ICustomer dalCustomer = Customer.Create(Settings.DAL);
                        dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);

                        //Now you start TX 
                        dalCustomer.BeginADOTransaction();
                        try
                        {
                            newCustomer = addNewRegisteredUser(dalCustomer, userID, password, fullname, address, email, creditcard, openBalance);
                            //done, commit.
                            dalCustomer.CommitADOTransaction();
                        }
                        catch
                        {
                            //explicit rollback needed.
                            dalCustomer.RollBackTransaction();
                            throw;
                        }
                        finally
                        {
                            //ALWAYS call dal.Close is using StockTrader DAL implementation;
                            //this is equivalent to calling Connection.Close() in the DAL --
                            //but for a generic DB backend as far as the BSL is concerned.
                            dalCustomer.Close();
                        }

                        break;
                    }

                default:
                    throw new Exception(Settings.ENABLE_GLOBAL_SYSTEM_DOT_TRANSACTIONS_CONFIGSTRING + ": " + StockTraderUtility.EXCEPTION_MESSAGE_INVALID_TXMMODEL_CONFIG + " Repository ConfigKey table.");
            }

            return newCustomer;
        }

        /// <summary>
        /// Register a client device with the news service.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public DeviceDataModel registerDevice(string userID, string deviceID)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]{20,50}$");
            if (!regex.IsMatch(deviceID))
            {
                throw new ArgumentOutOfRangeException("deviceID", "Device ID must be 20 - 50 characters in length and consist of only letters and numbers.");
            }

            DeviceDataModel result = null;

            // Create the user account for the Service Bus (based on device ID)
            ServiceBusHelper serviceBusHelper = new ServiceBusHelper(Settings.NEWS_NAMESPACE, Settings.NEWS_MANAGEMENT_SECRET);
            string key = serviceBusHelper.CreateUser(deviceID, userID);

            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            try
            {
                dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);

                // Find which topic we should be using. The database maintains a count of the number of subscriptions that currently
                // exist for each topic. Querying the topic for this information is too slow (as the API retrieves a lot more than count).
                int? topicIndex = dalCustomer.getAccountDeviceTopicIndex(userID, deviceID);
                bool found = topicIndex.HasValue;

                // If the user doesn't have a topic, create one in the database.
                if (!found)
                {
                    topicIndex = dalCustomer.getNewAccountDeviceTopicIndex();
                    dalCustomer.insertAccountDeviceTopicIndex(userID, deviceID, topicIndex.Value);
                }

                string topic = topicIndex > 0 ? string.Format(CultureInfo.InvariantCulture, "{0}-{1}", Settings.NEWS_TOPIC, topicIndex) : Settings.NEWS_TOPIC;

                // If the user doesn't have a topic, create one in the service bus.
                if (!found)
                {
                    serviceBusHelper.CreateTopic(topic);
                    serviceBusHelper.CreateSubscription(topic, deviceID);
                    serviceBusHelper.CreateRuleForUser(topic, deviceID, deviceID);
                }

                result = new DeviceDataModel(serviceBusHelper.GetSubscriptionUri(topic, deviceID).ToString(), deviceID, key);
            }
            finally
            {
                dalCustomer.Close();
            }


            return result;
        }

        /// <summary>
        /// Gets a holding for a user.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        /// <param name="holdingID">Holding id to retrieve data for.</param>
        /// <returns></returns>
        public HoldingDataModel getHolding(string userID, string holdingID)
        {
            HoldingDataModel holding = null;

            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            
            try
            {
                holding = dalCustomer.getHolding(userID, holdingID);
            }
            finally
            {
                dalCustomer.Close();
            }

            if (holding != null)
            {
                IQuoteService client = GetQuoteService(Settings.QUOTE_MODE, new Settings());
                QuoteDataModel quoteDataModel = client.GetQuote(holding.quoteID);
                if (quoteDataModel != null)
                {
                    holding.price = quoteDataModel.price;
                }
            }

            return holding;
        }

        /// <summary>
        /// Gets holding data for a user.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        /// <returns>Collection of holdings.</returns>
        public List<HoldingDataModel> getHoldings(string userID)
        {
            List<HoldingDataModel> holdings = null;

            ICustomer dalCustomer = Customer.Create(Settings.DAL);
            dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
            
            try
            {
                holdings = dalCustomer.getHoldings(userID);
            }
            finally
            {
                dalCustomer.Close();
            }

            if (holdings.Count > 0)
            {
                IQuoteService client = GetQuoteService(Settings.QUOTE_MODE, new Settings());
                List<QuoteDataModel> quotes = client.GetQuotes(string.Join(",", holdings.Select(x => x.quoteID).Distinct()));
                quotes.ToList().ForEach(x => holdings.Where(y => y.quoteID == x.symbol).ToList().ForEach(y => y.price = x.price));
            }

            return holdings;
        }

        /// <summary>
        /// Gets a quote for a given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>Quote for symbol.</returns>
        public QuoteDataModel getQuote(string symbol)
        {
            IQuoteService client = GetQuoteService(Settings.QUOTE_MODE, new Settings());
            return client.GetQuote(symbol);
        }

        /// <summary>
        /// Gets quotes for a collection of symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns>Quotes for symbols.</returns>
        public List<QuoteDataModel> getQuotes(string symbols)
        {
            IQuoteService client = GetQuoteService(Settings.QUOTE_MODE, new Settings());
            return client.GetQuotes(symbols);
        }

        static MarketSummaryDataModelWS tempReturnData = null;
        static bool gettingMarketSummaryData = false;
        /// <summary>
        /// Gets the market summary.
        /// </summary>
        /// <returns>Market summary.</returns>
        public MarketSummaryDataModelWS getMarketSummary()
        {
            MarketSummaryDataModelWS returnData = null;
            returnData = (MarketSummaryDataModelWS)HttpRuntime.Cache["mktSummary"];
            if (returnData == null)
            {
                if (gettingMarketSummaryData && tempReturnData != null)
                    returnData = tempReturnData;
                else
                {
                    try
                    {
                        gettingMarketSummaryData = true;
                        IQuoteService client = GetQuoteService(Settings.QUOTE_MODE, new Settings());
                        returnData = client.GetMarketSummaryData();
                        HttpRuntime.Cache.Insert("mktSummary", returnData, null, System.DateTime.UtcNow.AddSeconds(60), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (returnData != null)
                            tempReturnData = returnData;
                        gettingMarketSummaryData = false;
                    }
                }
            }
            return returnData;
        }

        /// <summary>
        /// Closes the order after being processed by the OPS.
        /// </summary>
        /// <param name="orderResponse"></param>
        public void OrderProcessed(OrderResponseDataModel orderResponse)
        {
            // Check connection is valid.
            CheckConnection();

            IOrder dalOrder = null;

            try
            {
                dalOrder = Order.Create(Settings.DAL);
                dalOrder.Open(Settings.TRADEDB_SQL_CONN_STRING);

                orderProcessed(dalOrder, orderResponse);
            }
            finally
            {
                if (dalOrder != null) dalOrder.Close();
            }
        }

        /// <summary>
        /// Places the order.
        /// </summary>
        /// <param name="orderType">Type of the order.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Order model.</returns>
        private OrderDataModel placeOrder(string orderType, string userID, string holdingID, string symbol, double quantity)
        {
            OrderDataModel order = null;
            HoldingDataModel holding = new HoldingDataModel();

            switch (Settings.TRANSACTION_MODEL)
            {
                case (StockTraderUtility.TRANSACTION_MODEL_SYSTEMDOTTRANSACTION_TRANSACTION):
                    {
                        CheckConnection();

                        IOrder dalOrder = Order.Create(Settings.DAL);
                        dalOrder.Open(Settings.TRADEDB_SQL_CONN_STRING);

                        try
                        {
                            //Start our System.Transactions tx with the options set above.
                            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions()))
                            {
                                //Business Step 1:  create the order header.
                                order = createOrder(dalOrder, orderType, userID, holdingID, symbol, quantity, ref holding);

                                //Business Step 2:  Determine which order processing mode to use.
                                //Fire up our async client;  we follow the same model here as with the
                                //StockTrader Web App in that we do not talk 'directly' to the generated proxy
                                //for the service; rather we channel all calls through a single 
                                //class that then talks to the service proxy.  This will aid in more
                                //easily knowing where communication and proxy logic sits; and make changes to services
                                //you might want to interface with vs. littering proxy calls throughout the
                                //business tier itself.
                                IOrderProcessor asyncclient = GetOrderProcessor(Settings.ORDER_PROCESSING_MODE, dalOrder);
                                using (new TransactionScope(TransactionScopeOption.Suppress))
                                {
                                    // Txns are supported by queue but can't promote existing txn
                                    asyncclient.SubmitOrder(order);
                                }

                                //Commit!
                                tx.Complete();
                            }
                        }
                        finally
                        {
                            dalOrder.Close();
                        }

                        break;
                    }

                //Repeat for ADO.NET transactions config option case. 
                case (StockTraderUtility.TRANSACTION_MODEL_ADONET_TRANSACTION):
                    {
                        IOrder dalOrder = Order.Create(Settings.DAL);
                        dalOrder.Open(Settings.TRADEDB_SQL_CONN_STRING);
                        dalOrder.BeginADOTransaction();
                        try
                        {
                            //Business Step 1:  create the order header.
                            order = createOrder(dalOrder, orderType, userID, holdingID, symbol, quantity, ref holding);

                            //Business Step 2:  Determine which order processing mode to use.
                            //Fire up our async client;  we follow the same model here as with the
                            //StockTrader Web App in that we do not talk 'directly' to the generated proxy
                            //for the service; rather we channel all calls through a single 
                            //class that then talks to the service proxy.  This will aid in more
                            //easily knowing where communication and proxy logic sits; and make changes to services
                            //you might want to interface with vs. littering proxy calls throughout the
                            //business tier itself.
                            IOrderProcessor asyncclient = GetOrderProcessor(Settings.ORDER_PROCESSING_MODE, dalOrder);
                            asyncclient.SubmitOrder(order);
                            dalOrder.CommitADOTransaction();
                        }
                        catch
                        {
                            dalOrder.RollBackTransaction();
                            throw;
                        }
                        finally
                        {
                            dalOrder.Close();
                        }

                        break;
                    }
                default:
                    throw new Exception(Settings.ENABLE_GLOBAL_SYSTEM_DOT_TRANSACTIONS_CONFIGSTRING + ": " + StockTraderUtility.EXCEPTION_MESSAGE_INVALID_TXMMODEL_CONFIG + " Repository ConfigKey table.");
            }

            return order;
        }

        /// <summary>
        /// This short try/catch block is introduced to deal with idle-timeout on SQL Azure
        /// connections.  It may not be required in the near future, but as of publication
        /// SQL Azure disconnects idle connections after 30 minutes.  While command retry-logic
        /// in the DAL automatically deals with this, when performing a tx, with the BSL handling
        /// tx boundaries, we want to go into the tx with known good connections.  The try/catch below
        /// ensures this.
        /// </summary>
        private static void CheckConnection()
        {
            ICustomer dalCustomer = null;
            try
            {
                dalCustomer = Customer.Create(Settings.DAL);
                dalCustomer.Open(Settings.TRADEDB_SQL_CONN_STRING);
                dalCustomer.getSQLContextInfo();
            }
            catch
            {
            }
            finally
            {
                if (dalCustomer != null)
                {
                    dalCustomer.Close();
                }
            }
        }

        /// <summary>
        /// Update order after processing by the OPS is finished.
        /// </summary>
        /// <param name="dalOrder"></param>
        /// <param name="orderResponse"></param>
        private static void orderProcessed(IOrder dalOrder, OrderResponseDataModel orderResponse)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions()))
            {
                // Retrieve the order to be completed
                OrderDataModel order = dalOrder.getOrder(orderResponse.userID, orderResponse.orderID);
                if (order == null)
                {
                    throw new InvalidOperationException("Unable to complete order as order not found: " + orderResponse.orderID);
                }

                // Retrieve the holding related to the order
                HoldingDataModel holding = dalOrder.getHolding(orderResponse.userID, order.holdingID);

                // Close the order/holding
                closeOrder(dalOrder, orderResponse.userID, order, holding, orderResponse.price);

                tx.Complete();
            }
        }

        /// <summary>
        /// Get the standard set of transaction options.
        /// </summary>
        private static TransactionOptions GetTransactionOptions()
        {
            return new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(Settings.SYSTEMDOTTRANSACTION_TIMEOUT)
            };
        }

        /// <summary>
        /// Business logic to synchronously close the order within BSL layer.
        /// </summary>
        private static void closeOrder(IOrder dalOrder, string userID, OrderDataModel order, HoldingDataModel holding, decimal price)
        {
            decimal total = 0;
            string holdingid = null;

            //Get the latest trading price--this is the money going into (or out of) the users account.
            order.price = price;

            //Calculate order total, and create/update the Holding. Whole holding 
            //sells delete the holding, partials sells update the holding with new amount
            //(and potentially the order too), and buys create a new holding.
            if (order.orderType == StockTraderUtility.ORDER_TYPE_BUY)
            {
                holdingid = dalOrder.createHolding(order);
                total = Convert.ToDecimal(order.quantity) * order.price + order.orderFee;
            }
            else
            {
                if (order.orderType == StockTraderUtility.ORDER_TYPE_SELL)
                {
                    holdingid = sellHolding(dalOrder, order, holding);
                    total = -1 * Convert.ToDecimal(order.quantity) * order.price + order.orderFee;
                }
            }

            //Debit/Credit User Account.  Note, if we did not want to allow unlimited margin
            //trading, we would change the ordering a bit and add the biz logic here to make
            //sure the user has enough money to actually buy the shares they asked for!

            //Now Update Account Balance.
            dalOrder.updateAccountBalance(userID, total);

            //Now perform our ACID tx test, if requested based on order type and acid stock symbols
            if (order.symbol.Equals(StockTraderUtility.ACID_TEST_BUY) && order.orderType == StockTraderUtility.ORDER_TYPE_BUY)
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_ACID_BUY);
            if (order.symbol.Equals(StockTraderUtility.ACID_TEST_SELL) && order.orderType == StockTraderUtility.ORDER_TYPE_SELL)
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_ACID_SELL);

            //Finally, close the order
            order.orderStatus = StockTraderUtility.ORDER_STATUS_CLOSED;
            order.completionDate = DateTime.Now;
            order.holdingID = holdingid;
            dalOrder.closeOrder(order);
            //Done!
        }

        /// <summary>
        /// Business logic to create the order header.
        /// The order header is always created synchronously by Trade; its the actual
        /// processing of the order that can be done asynchrounously via the WCF service.
        /// If, however, this service cannot communicate with the async order processor,
        /// the order header is rolled back out of the database since we are wrapped in a tx here
        /// either ADO.NET tx or System.TransactionScope as noted above, based on user setting.
        /// </summary>
        private static OrderDataModel createOrder(IOrder dalOrder, string orderType, string userID, string holdingID, string symbol, double quantity, ref HoldingDataModel holding)
        {
            OrderDataModel order;
            switch (orderType)
            {
                case StockTraderUtility.ORDER_TYPE_SELL:
                    {
                        holding = dalOrder.getHolding(userID, holdingID);
                        if (holding == null)
                            throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_INVALID_HOLDING_NOT_FOUND);
                        order = dalOrder.createOrder(userID, holding.quoteID, StockTraderUtility.ORDER_TYPE_SELL, holding.quantity, holdingID);
                        break;
                    }
                //StockTrader 5 allows users to sell part
                //of a holding, not required to sell all shares at once.  This business logic
                //on the processing side of the pipe is more tricky.  Have to check for 
                //conditions like another order coming in and the shares do not exist anymore 
                //in the holding, etc.  This is not done here--it is done in the ProcessOrder class.
                //Here we are merely creatingt he order header as with all orders--note its just the *quantity* variable
                //that varies with SELL_ENHANCED case vs. SELL.  SELL always uses the total
                //number of shares in the holding to sell; here we have obtained from the
                //requesting service (for example, the StockTrader Web App), how many shares
                //the user actually wants to sell.
                case StockTraderUtility.ORDER_TYPE_SELL_ENHANCED:
                    {
                        holding = dalOrder.getHolding(userID, holdingID);
                        if (holding == null)
                            throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_INVALID_HOLDING_NOT_FOUND);
                        //If user requests to sell more shares than in holding, we will just invoke the core
                        //sell operation which liquidates/sells the entire holding. Seems logical--they will
                        //get proper notification based on the client order alert in the Web app how many
                        //were actually sold.
                        if (quantity > holding.quantity)
                        {
                            goto case StockTraderUtility.ORDER_TYPE_SELL;
                        }
                        
                        order = dalOrder.createOrder(userID, holding.quoteID, StockTraderUtility.ORDER_TYPE_SELL, quantity, holdingID);
                        break;
                    }
                case StockTraderUtility.ORDER_TYPE_BUY:
                    {
                        //Buys are easier business case!  Especially when on unlimited margin accounts :-).
                        order = dalOrder.createOrder(userID, symbol, StockTraderUtility.ORDER_TYPE_BUY, quantity, null);
                        break;
                    }
                default:
                    throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_BADORDERTYPE);
            }
            return order;
        }

        /// <summary>
        /// Adds user account data to Account table and also profile data to AccountProfile table.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="fullname"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="creditcard"></param>
        /// <param name="openBalance"></param>
        /// <returns></returns>
        private static AccountDataModel addNewRegisteredUser(ICustomer dalCustomer, string userID, string password, string fullname, string address, string email, string creditcard, decimal openBalance)
        {
            AccountProfileDataModel customerprofile = new AccountProfileDataModel(userID, password, fullname, address, email, creditcard);
            dalCustomer.insertAccountProfile(customerprofile, Settings.USE_SALTEDHASH_PASSWORDS);

            //Check our acid test conditions here for transactional testing; we want to test part way through
            //the register operations from the BSL, to make sure database is never left in state with one
            //insert above going through, and the one below not--the entire BSL operation needs to be
            //treated as one logical unit of work. Also note the ordering of operations here:
            //since trying to register a non-unique userid might be something that happens frequently in the real
            //world, lets do the insert that would fail on this condition first (accountprofile); 
            //rather than wait and do it last.
            if (customerprofile.userID.Equals(StockTraderUtility.ACID_TEST_USER))
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_ACID_REGISTRATION);
            AccountDataModel customer = new AccountDataModel(null, userID, DateTime.Now, openBalance, 0, openBalance, DateTime.Now, 0);
            dalCustomer.insertAccount(customer);
            return customer;
        }

        /// <summary>
        /// Sell the holding.
        /// </summary>
        /// <param name="dalOrder"></param>
        /// <param name="order"></param>
        /// <param name="holding"></param>
        /// <returns></returns>
        private static string sellHolding(IOrder dalOrder, OrderDataModel order, HoldingDataModel holding)
        {
            string holdingid = holding.holdingID;
            //There are three distinct business cases here, each needs different treatment:  
            // a) Quantity requested is less than total shares in the holding -- update holding.  
            // b) Quantity requested is equal to total shares in the holding -- delete holding.  
            // c) Quantity requested is greater than total shares in the holding -- delete holding, update order table.  
            if (order.quantity < holding.quantity)
            {
                dalOrder.updateHolding(order.userID, holdingid, order.quantity);
            }
            else
            {
                if (holding.quantity == order.quantity)
                {
                    dalOrder.deleteHolding(order.userID, holdingid);
                }
                else
                {
                    //We now need to back-update the order record quantity to reflect
                    //fact not all shares originally requested were sold since the holding 
                    //had less shares in it, perhaps due to other orders 
                    //placed against that holding that completed before this one. So we will
                    //sell the remaining shares, but need to update the final order to reflect this.
                    if (order.quantity > holding.quantity)
                    {
                        dalOrder.deleteHolding(order.userID, holdingid);
                        order.quantity = holding.quantity;
                        dalOrder.updateOrder(order);
                    }
                }
            }

            return holdingid;
        }

        /// <summary>
        /// Return a quote service accessor depending on the quote mode.
        /// </summary>
        private static IQuoteService GetQuoteService(string quoteMode, object settingsInstance)
        {
            return quoteMode == StockTraderUtility.QS_INPROCESS ? (IQuoteService) new QuoteService() : new QuoteServiceClient(quoteMode, settingsInstance);
        }

        /// <summary>
        /// Return a order processor accessor depending on the order mode.
        /// </summary>
        private static IOrderProcessor GetOrderProcessor(string orderMode, IOrder dalOrder)
        {
            IOrderProcessor orderProcessor = null;
            if (orderMode == StockTraderUtility.OPS_INPROCESS)
            {
                orderProcessor = new OrderProcessorEngine();
                ((OrderProcessorEngine)orderProcessor).OrderProcessed += (s, e) => orderProcessed(dalOrder, e.Response);
            }
            else
            {
                orderProcessor = new TradeOrderServiceAsyncClient(orderMode, new Settings());
            }

            return orderProcessor;
        }
    }
}
