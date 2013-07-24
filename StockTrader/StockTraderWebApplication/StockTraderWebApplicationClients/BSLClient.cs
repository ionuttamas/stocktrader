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
// The WCF client to the BSL.
//======================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConfigService.ServiceConfigurationUtility;
using Trade.BusinessServiceContract;
using Trade.BusinessServiceDataContract;
using Trade.BusinessServiceImplementation;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationSettings;
using Trade.Utility;

namespace Trade.StockTraderWebApplicationServiceClient
{
    /// <summary>
    /// This is the business services client class that is called from StockTrader Web pages. It will actually
    /// call into our WCF BusinessServiceClient class, if configured for remote activation.  
    /// </summary>
    public class BSLClient 
    {

        ITradeServices BSL;

        /// <summary>
        /// Depending on AccessMode, this constructor returns an instance of our WCF client class, or
        /// a direct instance of our TradeServices implementation class.  
        /// </summary>
        public BSLClient()
        {
            switch (Settings.ACCESS_MODE)
            {
                //In-process activation---instantiate BSL directly, no need for client interface.
                case StockTraderUtility.BSL_INPROCESS: 
                    { 
                        BSL = new TradeServiceBSL();
                        break;
                    }

                //Remote activation.
                //Note the same WCF client is used regardless of the 
                //specific webservice implementation platform.  For StockTrader, our client
                //interface for SOA modes is always the same:  BusinessServiceClient.
                //We differentiate WebSphere only becuase StockTrader has some additional UI and
                //backend service functionality not provided by J2EE/Trade 6.1, and we need to detect
                //in just a couple of places in the Web app so we do not make method calls to an 
                //implementation that has not implemented those methods. But as you see here, the 
                //WCF client is always the same regardless.
                default: 
                        {
                            BSL = new BusinessServiceClient(new Settings());
                            break;
                        }
            }
            
        }

        /// <summary>
        /// Logs user in/authenticates against StockTrader database.
        /// </summary>
        /// <param name="userid">User id to authenticate.</param>
        /// <param name="password">Password for authentication</param>
        public AccountDataUI login(string userid, string password)
        {
            try
            {
                AccountDataModel customer = BSL.login(userid, password);
                if (customer == null)
                    return null;
                return new AccountDataUI(customer.accountID, customer.profileID, customer.creationDate, customer.openBalance, customer.logoutCount, customer.balance, customer.lastLogin, customer.loginCount);
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.login Error: " + e.ToString(),EventLogEntryType.Error,true,new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets recent orders for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public TotalOrdersUI getOrders(string userID)
        {
            try
            {
                List<OrderDataModel> orders = BSL.getOrders(userID);
                List<OrderDataUI> ordersUI = new List<OrderDataUI>();
                decimal subtotalSell = 0;
                decimal subtotalBuy = 0;
                decimal subtotalTxnFee = 0;
                for (int i = 0; i < orders.Count; i++)
                 {
                     subtotalTxnFee += orders[i].orderFee;
                     if (orders[i].orderType == StockTraderUtility.ORDER_TYPE_SELL)
                     {
                         subtotalSell += orders[i].price * (decimal)orders[i].quantity - orders[i].orderFee;
                     }
                     else
                     {
                         subtotalBuy += orders[i].price * (decimal) orders[i].quantity + orders[i].orderFee;
                     }
                     ordersUI.Add((convertOrderToUI(orders[i])));
                 }
                 TotalOrdersUI totalOrders = new TotalOrdersUI(ordersUI, subtotalSell, subtotalBuy, subtotalTxnFee);
                 return totalOrders;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getOrders Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets specific top n orders for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public TotalOrdersUI getTopOrders(string userID)
        {
            try
            {
                List<OrderDataModel> orders = BSL.getTopOrders(userID);
                List<OrderDataUI> ordersUI = new List<OrderDataUI>();
                decimal subtotalSell = 0;
                decimal subtotalBuy = 0;
                decimal subtotalTxnFee = 0;
                for (int i = 0; i < orders.Count; i++)
                {
                    subtotalTxnFee += orders[i].orderFee;
                    if (orders[i].orderType == StockTraderUtility.ORDER_TYPE_SELL)
                    {
                        subtotalSell += orders[i].price * (decimal)orders[i].quantity - orders[i].orderFee;
                    }
                    else
                    {
                        subtotalBuy += orders[i].price * (decimal)orders[i].quantity + orders[i].orderFee;
                    }
                    ordersUI.Add((convertOrderToUI(orders[i])));
                }
                TotalOrdersUI totalOrders = new TotalOrdersUI(ordersUI, subtotalSell, subtotalBuy, subtotalTxnFee);
                return totalOrders;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getTopOrders Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets account data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public AccountDataUI getAccountData(string userID)
        {
            try
            {
                AccountDataModel customer = BSL.getAccountData(userID);
                return new AccountDataUI(customer.accountID, customer.profileID, customer.creationDate, customer.openBalance, customer.logoutCount, customer.balance, customer.lastLogin, customer.loginCount);
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getAccountData Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets account profile data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public AccountProfileDataUI getAccountProfileData(string userID)
        {
            try
            {
                AccountProfileDataModel customerprofile = BSL.getAccountProfileData(userID);
                return new AccountProfileDataUI(userID, customerprofile.password, customerprofile.fullName, customerprofile.address, customerprofile.email, customerprofile.creditCard);
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getAccountProfileData Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Updates account profile data for a user. 
        /// </summary>
        /// <param name="customerprofile">Profile data model class with updated info.</param>
        public AccountProfileDataUI updateAccountProfile(AccountProfileDataUI customerprofile)
        {
            try
            {
                AccountProfileDataModel serviceLayerCustomerProfile = convertCustProfileFromUI(customerprofile);
                serviceLayerCustomerProfile = BSL.updateAccountProfile(serviceLayerCustomerProfile);
                return new AccountProfileDataUI(serviceLayerCustomerProfile.userID, serviceLayerCustomerProfile.password, serviceLayerCustomerProfile.fullName, serviceLayerCustomerProfile.address, serviceLayerCustomerProfile.email, serviceLayerCustomerProfile.creditCard);
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.updateAccountProfile Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets holding data for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public Trade.StockTraderWebApplicationModelClasses.TotalHoldingsUI getHoldings(string userID)
        {
            try
            {
                List<HoldingDataModel> holdings = BSL.getHoldings(userID);
                List<HoldingDataUI> holdingsUI = new List<HoldingDataUI>();
                decimal marketValue = 0;
                decimal gain = 0;
                decimal basis = 0;
                for (int i = 0; i < holdings.Count; i++)
                {
                    //Now removed: QuoteDataModel quote = BSL.getQuote(holdings[i].quoteID);

                    HoldingDataUI holdingitem = new HoldingDataUI(holdings[i].holdingID, holdings[i].quantity, holdings[i].purchasePrice, holdings[i].purchaseDate.ToString(), holdings[i].quoteID, holdings[i].price);
                    holdingitem.convertNumericsForDisplay(false);
                    holdingsUI.Add(holdingitem);
                    decimal _marketValue = (decimal)holdings[i].quantity * holdings[i].price;
                    decimal _basis = (decimal)holdings[i].quantity * (decimal)holdings[i].purchasePrice;
                    gain += _marketValue - _basis;
                    marketValue += _marketValue;
                    basis += _basis;
                }
                TotalHoldingsUI totalHoldings = new TotalHoldingsUI(holdingsUI, marketValue, basis, gain);
                return totalHoldings;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getHoldings Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        ///       This routine allows us to take an unsorted list (or list sorted by HoldingID) of holdings, 
        ///       such as that returned by the WebSphere Trade 6.1 service, and return a sorted list of 
        ///       holdings by stock symbol. At the same time, we produce subtotal lines for each unique stock. 
        ///       This is used in a new page we added for StockTrader that Trade 6.1 does not have:
        ///       (PortfolioBySymbol.aspx).  Yet, it will work with the existing WebSphere backend service,  
        ///       since we do the sort here on the UI tier. We do it this way becuase WebSphere Trade 6.1
        ///       will always return an unsorted list of stock holdings since it does not implement a web service
        ///       method to return a list sorted by quoteID. Without this limiting factor, a better and more 
        ///       performant way of doing this would be to implement a business services method and corresponding DAL method
        ///       to execute a query that returned pre-sorted values by stock symbol (as opposed to unsorted, 
        ///       as the EJB/Trade 6.1 getHoldings operation does; or sorted by HoldingID as our getHoldings operation
        ///       does.  This would avoid the need to do a sort on the UI tier at all.  The extra load this 
        ///       would place on the database would be negligable, given a typically small number of rows returned.    
        ///       At any rate, the sort is implemented here by taking advantage of a custom comparer in our
        ///       HoldingsUI class on the quoteID property (stock symbol), and .NET's ability to sort 
        ///       generic typed lists.
        ///       Hence, we can display even WebSphere-returned data that is non sorted in our extra 
        ///       PortfolioBySymbol page, adding the subtotal lines for each unique stock as well.
        ///       We resisted adding a second method to the BSL and DAL to return a different sort order (quoteID), simply to 
        ///       be consistent across the InProcess and Web Service modes of operation and for true benchmark comparison 
        ///       purposes between the InProcess and web service modes.  Note this logic is never called in 
        ///       published benchmark data, given the fact WebSphere Trade 6.1 does not implement this 
        ///       functionality, although even with the extra sort it performs very well.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public Trade.StockTraderWebApplicationModelClasses.TotalHoldingsUI getHoldingsBySymbolSubTotaled(string userID)
        {
            try
            {
                //get the list of holdings from the BSL
                List<HoldingDataModel> holdings = BSL.getHoldings(userID);
                List<HoldingDataUI> holdingsUI = new List<HoldingDataUI>();
                decimal marketValue = 0;
                decimal gain = 0;
                decimal basis = 0;
                //create our HoldingsUI class to pass back to the ASPX page.
                for (int i = 0; i < holdings.Count; i++)
                {
                    //Now Removed: QuoteDataModel quote = BSL.getQuote(holdings[i].quoteID);
                    HoldingDataUI holdingitem = new HoldingDataUI(holdings[i].holdingID, holdings[i].quantity, holdings[i].purchasePrice, holdings[i].purchaseDate.ToString(), holdings[i].quoteID, holdings[i].price);
                    holdingsUI.Add(holdingitem);
                    decimal _marketValue = (decimal)holdings[i].quantity * holdings[i].price;
                    decimal _basis = (decimal)holdings[i].quantity * holdings[i].purchasePrice;
                    gain += _marketValue - _basis;
                    marketValue += _marketValue;
                    basis += _basis;
                }
                //Call our implemented comparer class: see Trade.StockTraderWebApplicationModelClasses.HoldingDataUI.cs for the compararer
                //class source code.
                HoldingDataUI.HoldingDataUIComparer comparer = new HoldingDataUI.HoldingDataUIComparer();
                comparer.ComparisonMethod = HoldingDataUI.HoldingDataUIComparer.ComparisonType.quoteID;
                
                //Do the sort! Sort method is built into the C# Generic List functionality; the comparer
                //calls our delegate method to compare by quoteID, so just one line of code here!
                holdingsUI.Sort(comparer);

                //Our list is now sorted, proceed with building in the subtotal lines.
                htmlRowBuilder rowBuilder = new htmlRowBuilder();
                int uniqueStockCount = rowBuilder.buildPortfolioBySymbol(holdingsUI);
                TotalHoldingsUI totalHoldings = new TotalHoldingsUI(holdingsUI, marketValue, basis, gain, uniqueStockCount, holdingsUI.Count - uniqueStockCount);
                return totalHoldings;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getHoldingsBySymbolSubTotaled Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets a holding for a user.  Transforms data from DataContract to model UI class for HTML display.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        /// <param name="holdingid">Holding id to retrieve data for.</param>
        public HoldingDataUI getHolding(string userID, string holdingid)
        {
            HoldingDataModel holding=null;
            HoldingDataUI holdingitem=null;
            try
            {
                holding = BSL.getHolding(userID, holdingid);
                if (holding!=null)
                {
                    QuoteDataModel quote = BSL.getQuote(holding.quoteID);
                    holdingitem = new HoldingDataUI(holding.holdingID, holding.quantity, holding.purchasePrice, holding.purchaseDate, holding.quoteID, quote.price);
                }
                return holdingitem;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getHolding Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
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
                BSL.logout(userID);
                return;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.logout Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
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
        /// <param name="openBalance">Open balance as specified by user. Might as well make it lots of $!</param>
        public AccountDataUI register(string userID, string password, string fullname, string address, string email, string creditcard, decimal openBalance)
        {
            try
            {
                AccountDataModel customer = BSL.register(userID, password, fullname, address, email, creditcard, openBalance);
                return new AccountDataUI(customer.accountID, customer.profileID, customer.creationDate, customer.openBalance, customer.logoutCount, customer.balance, customer.lastLogin, customer.loginCount);
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.register Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets any closed orders for a user--orders that have been processed.  Also updates status to complete.
        /// </summary>
        /// <param name="userID">User id to retrieve data for.</param>
        public List<Trade.StockTraderWebApplicationModelClasses.OrderDataUI> getClosedOrders(string userID)
        {
            try
            {
                List<OrderDataUI> ordersUI = new List<OrderDataUI>();
                List<OrderDataModel> orders = BSL.getClosedOrders(userID);
                for (int i = 0; i < orders.Count; i++)
                {
                    ordersUI.Add((convertOrderToUI(orders[i])));
                }
                return ordersUI;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getClosedOrders Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Performs a stock buy operation.
        /// </summary>
        /// <param name="userID">User id to create/submit order for.</param>
        /// <param name="symbol">Stock symbol to buy.</param>
        /// <param name="quantity">Shares to buy</param>
        public OrderDataUI buy(string userID, string symbol, double quantity)
        {
            try
            {
                return convertOrderToUI(BSL.buy(userID, symbol, quantity, 0));
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.buy Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Performs a holding sell operation.
        /// </summary>
        /// <param name="userID">User id to create/submit order for.</param>
        /// <param name="holdingID">Holding id to sell off.</param>
        /// <param name="quantity">Shares to sell.</param>
        public OrderDataUI sell(string userID, string holdingID, double quantity)
        {
            try
            {
                if (quantity==0)
                    return convertOrderToUI(BSL.sell(userID, holdingID, 0));
                else
                    return convertOrderToUI(BSL.sellEnhanced(userID, holdingID, quantity));
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.sell Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets a list of stock quotes based on symbols.
        /// </summary>
        /// <param name="symbols">Symbols to get data for.</param>
        public List<QuoteDataUI> getQuotes(string symbols)
        {
            try
            {
                List<QuoteDataModel> quotes = BSL.getQuotes(symbols);
                List<QuoteDataUI> quoteList = new List<QuoteDataUI>();
                foreach (QuoteDataModel quote in quotes)
                {
                   QuoteDataUI quoteDataUI = convertQuoteToUI(quote);
                   quoteList.Add(quoteDataUI);
                }
                return quoteList;
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getQuotes Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets a single quote based on symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get data for.</param>
        public QuoteDataUI getQuote(string symbol)
        {
            try
            {
                return convertQuoteToUI(BSL.getQuote(symbol));
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getQuote Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Gets the current market summary.  This results in an expensive DB query in the DAL; hence look to cache data returned for 60 second or so.
        /// </summary>
        public Trade.StockTraderWebApplicationModelClasses.MarketSummaryDataUI getMarketSummary()
        {
            try
            {
                return convertMarketSummaryDataToUI(BSL.getMarketSummary());
            }
            catch (Exception e)
            {
                ConfigUtility.writeErrorConsoleMessage("StockTraderWebApplicationServiceClient.getMarketSummary Error: " + e.ToString(), EventLogEntryType.Error, true, new Settings());
                throw;
            }
        }

        /// <summary>
        /// Converts from service data contract model class to a UI Model class for quick HTML display in ASPX pages.
        /// </summary>
        private QuoteDataUI convertQuoteToUI(QuoteDataModel quote)
        {
            if (quote != null)
                return new QuoteDataUI(quote.symbol, quote.companyName, quote.volume, quote.price, quote.open, quote.low, quote.high, quote.change);
            else return null;
        }

        /// <summary>
        /// Converts from service data contract model class to a UI Model class for quick HTML display in ASPX pages.
        /// </summary>
        private Trade.StockTraderWebApplicationModelClasses.MarketSummaryDataUI convertMarketSummaryDataToUI(MarketSummaryDataModelWS data)
        {
            List<QuoteDataUI> quoteGainers = new List<QuoteDataUI>();
            List<QuoteDataUI> quoteLosers = new List<QuoteDataUI>();
            for (int i = 0; i < data.topGainers.Count; i++)
            {
                QuoteDataModel quote = (QuoteDataModel)data.topGainers[i];
                quoteGainers.Add((convertQuoteToUI(quote)));
            }
            for (int i = 0; i < data.topLosers.Count; i++)
            {
                QuoteDataModel quote = (QuoteDataModel)data.topLosers[i];
                quoteLosers.Add((convertQuoteToUI(quote)));
            }
            return new MarketSummaryDataUI(data.TSIA, data.openTSIA, data.volume, quoteGainers, quoteLosers, data.summaryDate);
        }

        /// <summary>
        /// Converts from service data contract model class to a UI Model class for quick HTML display in ASPX pages.
        /// </summary>
        private OrderDataUI convertOrderToUI(OrderDataModel order)
        {
            string completionDate;
            if (order != null)
            {
                //MinValue used to indicate "null" data in the DB; equates to a pending order.
                if (order.completionDate == DateTime.MinValue)
                    completionDate = "Pending";
                else
                    completionDate = order.completionDate.ToString();
                return new OrderDataUI(order.orderID, order.orderType, order.orderStatus, order.openDate, completionDate, order.quantity, order.price, order.orderFee, order.symbol);
            }
            else 
                return null;
        }

        /// <summary>
        /// Converts from service data contract model class to a UI Model class for quick HTML display in ASPX pages.
        /// </summary>
        private AccountDataModel convertAccountDataFromUI(AccountDataUI customer)
        {
            AccountDataModel serviceLayerCustomer = new AccountDataModel();
            serviceLayerCustomer.accountID = customer.accountID;
            serviceLayerCustomer.balance = customer.balance;
            serviceLayerCustomer.creationDate = customer.creationDate;
            serviceLayerCustomer.lastLogin = customer.lastLogin;
            serviceLayerCustomer.logoutCount = customer.logoutCount;
            serviceLayerCustomer.openBalance = customer.openBalance;
            serviceLayerCustomer.profileID = customer.profileID;
            return serviceLayerCustomer;
        }

        /// <summary>
        /// Converts from service data contract model class to a UI Model class for quick HTML display in ASPX pages.
        /// </summary>
        private AccountProfileDataModel convertCustProfileFromUI(AccountProfileDataUI customerprofile)
        {
            AccountProfileDataModel serviceLayerCustomerProfile = new AccountProfileDataModel();
            serviceLayerCustomerProfile.password = customerprofile.password;
            serviceLayerCustomerProfile.address = customerprofile.address;
            serviceLayerCustomerProfile.creditCard = customerprofile.creditCard;
            serviceLayerCustomerProfile.email = customerprofile.email;
            serviceLayerCustomerProfile.fullName = customerprofile.fullName;
            serviceLayerCustomerProfile.userID = customerprofile.userID;
            return serviceLayerCustomerProfile;
        }
    }
}