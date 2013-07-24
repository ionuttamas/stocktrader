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
using System.Data;
using System.Data.SqlClient;
using ConfigService.DALSQLHelper;
using Trade.BusinessServiceDataContract;
using Trade.IDAL;
using Trade.Utility;

namespace Trade.DALSQLAzure
{
    /// <summary>
    /// Order implementation for SQL Azure.
    /// </summary>
    public class Order : IOrder
    {
        private const string SQL_INSERT_ORDER = "SET NOCOUNT ON; INSERT INTO dbo.ORDERS (ORDERID, OPENDATE, ORDERFEE, PRICE, QUOTE_SYMBOL, QUANTITY, ORDERTYPE, ORDERSTATUS, ACCOUNT_ACCOUNTID, PROFILE_USERIDHASH, HOLDING_HOLDINGID) VALUES (@OrderId, GETDATE(), @OrderFee, @Price, @QuoteSymbol, @Quantity, @OrderType, 'open', @accountId, @userIdHash, @HoldingId);";
        private const string SQL_SELECT_ORDER_BY_ID = "SET NOCOUNT ON; SELECT ORDERID, ORDERTYPE, ORDERSTATUS, OPENDATE, COMPLETIONDATE, QUANTITY, PRICE, ORDERFEE, QUOTE_SYMBOL, HOLDING_HOLDINGID from dbo.orders where account_accountid = (select accountid from dbo.account WITH (NOLOCK) where profile_userid = @UserId) and ORDERID = @OrderId";
        private const string SQL_GET_ACCOUNTID = "SET NOCOUNT ON; SELECT ACCOUNTID FROM dbo.ACCOUNT WHERE PROFILE_USERID = @userId";
        private const string SQL_GET_ACCOUNTID_ORDER = "SET NOCOUNT ON; SELECT ACCOUNT_ACCOUNTID FROM dbo.ORDERS WHERE ORDERID=@OrderId";
        private const string SQL_INSERT_HOLDING = "SET NOCOUNT ON; INSERT INTO dbo.HOLDING (PURCHASEPRICE, QUANTITY, PURCHASEDATE, ACCOUNT_ACCOUNTID, QUOTE_SYMBOL, PROFILE_USERIDHASH, HOLDINGID) VALUES (@PurchasePrice, @Quantity, @PurchaseDate, @AccountId, @QuoteSymbol, @UserIdHash, @HoldingId);";
        private const string SQL_SELECT_HOLDING = "SET NOCOUNT ON; SELECT HOLDING.HOLDINGID, HOLDING.QUANTITY, HOLDING.PURCHASEPRICE, HOLDING.PURCHASEDATE, HOLDING.QUOTE_SYMBOL, HOLDING.ACCOUNT_ACCOUNTID FROM dbo.HOLDING WITH (NOLOCK) WHERE HOLDINGID = @HoldingId";
        private const string SQL_DELETE_HOLDING = "SET NOCOUNT ON; DELETE FROM dbo.HOLDING WITH (ROWLOCK) WHERE HOLDINGID=@HoldingId";
        private const string SQL_UPDATE_HOLDING = "SET NOCOUNT ON; UPDATE dbo.HOLDING WITH (ROWLOCK) SET QUANTITY=QUANTITY-@Quantity WHERE HOLDINGID=@HoldingId";
        private const string SQL_UPDATE_ORDER = "SET NOCOUNT ON; UPDATE dbo.ORDERS WITH (ROWLOCK) SET QUANTITY=@Quantity WHERE ORDERID=@OrderId";
        private const string SQL_CLOSE_ORDER = "SET NOCOUNT ON; UPDATE dbo.ORDERS WITH (ROWLOCK) SET ORDERSTATUS = @status, COMPLETIONDATE=GetDate(), HOLDING_HOLDINGID=@HoldingId, PRICE=@Price WHERE ORDERID = @OrderId";

        //Parameters
        private const string PARM_SYMBOL = "@QuoteSymbol";
        private const string PARM_USERID = "@userId";
        private const string PARM_USERIDHASH = "@userIdHash";
        private const string PARM_ORDERSTATUS = "@status";
        private const string PARM_QUANTITY = "@Quantity";
        private const string PARM_ORDERTYPE = "@OrderType";
        private const string PARM_ACCOUNTID = "@accountId";
        private const string PARM_ORDERID = "@OrderId";
        private const string PARM_HOLDINGID = "@HoldingId";
        private const string PARM_ORDERFEE = "@OrderFee";
        private const string PARM_PRICE = "@Price";
        private const string PARM_PURCHASEPRICE = "@PurchasePrice";
        private const string PARM_PURCHASEDATE = "@PurchaseDate";

        //_internalConnection: Used by a DAL instance such that a DAL instance,
        //associated with a BSL instance, will work off a single connection between BSL calls.
        protected SqlConnection _internalConnection;

        //_internalADOTransaction: Used only when doing ADO.NET transactions.
        //This will be completely ignored when null, and not attached to a cmd object
        //In SQLHelper unless it has been initialized explicitly in the BSL with a
        //dal.BeginADOTransaction().  See app config setting in web.config and 
        //Trade.BusinessServiceHost.exe.config "Use System.Transactions Globally" which determines
        //whether user wants to run with ADO transactions or System.Transactions.  The DAL itself
        //is built to be completely agnostic and will work with either.
        protected SqlTransaction _internalADOTransaction;
        
        private readonly SqlAzureHelper _helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        public Order()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor for internal DAL-DAL calls to use an existing DB connection.
        /// </summary>
        protected internal Order(SqlConnection conn, SqlTransaction trans)
            : this(conn, trans, new SqlAzureHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        protected Order(SqlConnection conn, SqlTransaction trans, SqlAzureHelper helper)
        {
            _internalConnection = conn;
            _internalADOTransaction = trans;
            _helper = helper;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public virtual void BeginADOTransaction()
        {
            if (_internalConnection.State != ConnectionState.Open)
            {
                _internalConnection.Open();
            }

            getSQLContextInfo();
            _internalADOTransaction = _internalConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Used only when explicitly using ADO.NET transactions from the BSL.
        /// </summary>
        public virtual void RollBackTransaction()
        {
            if (_internalADOTransaction != null)
            {
                _internalADOTransaction.Rollback();
            }

            _internalADOTransaction = null;
        }

        /// <summary>
        /// Used only when explicitly using ADO.NET transactions from the BSL.
        /// </summary>
        public virtual void CommitADOTransaction()
        {
            if (_internalADOTransaction != null)
            {
                _internalADOTransaction.Commit();
            }

            _internalADOTransaction = null;
        }

        /// <summary>
        /// Opens the connection using the specified connnection string.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        public virtual void Open(string connString)
        {
            if (_internalConnection == null)
            {
                _internalConnection = _helper.Create(connString);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (_internalConnection != null && _internalConnection.State != ConnectionState.Closed)
            {
                _internalConnection.Close();
            }
        }

        /// <summary>
        /// Gets the SQL context info. Used to validate connection.
        /// </summary>
        public void getSQLContextInfo()
        {
            string sql = "SELECT 0";
            _helper.ExecuteScalarNoParm(_internalConnection, _internalADOTransaction, CommandType.Text, sql);
        }

        /// <summary>
        /// Gets the quote (for update).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public QuoteDataModel getQuoteForUpdate(string symbol)
        {
            IMarketSummary marketSummary = CreateMarketSummary(_internalConnection, _internalADOTransaction);
            return marketSummary.getQuoteForUpdate(symbol);
        }

        /// <summary>
        /// Updates the stock price volume.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="quote">The quote.</param>
        public void updateStockPriceVolume(double quantity, QuoteDataModel quote)
        {
            IMarketSummary marketSummary = CreateMarketSummary(_internalConnection, _internalADOTransaction);
            marketSummary.updateStockPriceVolume(quantity, quote);
        }

        /// <summary>
        /// Gets the holding for update.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="orderID">The order ID.</param>
        public HoldingDataModel getHoldingForUpdate(string userID, string orderID)
        {
            ICustomer customer = CreateCustomer(_internalConnection, _internalADOTransaction);
            return customer.getHoldingForUpdate(userID, orderID);
        }

        /// <summary>
        /// Updates the account balance for a user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="total">The total.</param>
        public void updateAccountBalance(string userID, decimal total)
        {
            ICustomer customer = CreateCustomer(_internalConnection, _internalADOTransaction);
            customer.updateAccountBalance(userID, total);
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="orderType">Type of the order.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="holdingID">The holding ID.</param>
        public virtual OrderDataModel createOrder(string userID, string symbol, string orderType, double quantity, string holdingID)
        {
            SqlParameter[] parm = new[] { new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20) };
            parm[0].Value = userID;

            // Retrieve the account ID (for the user ID)
            string accountId;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRow(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_GET_ACCOUNTID, parm))
            {
                rdr.Read();
                accountId = rdr.GetGuid(0).ToString();
            }

            // Create new order
            OrderDataModel order = new OrderDataModel(Guid.NewGuid().ToString(), orderType,
                                                      StockTraderUtility.ORDER_STATUS_OPEN, DateTime.Now,
                                                      DateTime.MinValue, quantity, 0,
                                                      StockTraderUtility.getOrderFee(orderType), symbol, userID)
                                       {holdingID = holdingID ?? Guid.NewGuid().ToString(), accountID = accountId};
            
            SqlParameter[] orderParms = GetCreateOrderParameters();
            orderParms[0].Value = order.orderFee;
            orderParms[1].Value = order.price;
            orderParms[2].Value = order.symbol;
            orderParms[3].Value = (float)order.quantity;
            orderParms[4].Value = order.orderType;
            orderParms[5].Value = Guid.Parse(order.accountID);
            orderParms[6].Value = Guid.Parse(order.holdingID);
            orderParms[7].Value = Guid.Parse(order.orderID);
            orderParms[8].Value = userID.GetSignedMurmur2HashCode();

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_INSERT_ORDER, orderParms);
            
            return order;
        }

        /// <summary>
        /// Gets the order by ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="orderID">The order ID.</param>
        public virtual OrderDataModel getOrder(string userID, string orderID)
        {
            OrderDataModel orderDataModel = null;

            SqlParameter useridparm = new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20)
                                          {Value = userID};
            SqlParameter orderidparm = new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier)
                                           {Value = Guid.Parse(orderID)};

            using (SqlDataReader rdr = _helper.ExecuteReader(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_ORDER_BY_ID, useridparm, orderidparm))
            {
                if (rdr.Read())
                {
                    Object completionDate;
                    //can be null
                    try
                    {
                        if (!Convert.IsDBNull(rdr.GetDateTime(4)))
                            completionDate = rdr.GetDateTime(4);
                        else
                            completionDate = DateTime.MinValue;
                    }
                    catch (Exception)
                    {
                        completionDate = DateTime.MinValue;
                    }

                    orderDataModel = new OrderDataModel(rdr.GetGuid(0).ToString(), rdr.GetString(1), rdr.GetString(2),
                                                        rdr.GetDateTime(3), (DateTime) completionDate, rdr.GetDouble(5),
                                                        rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetString(8), userID)
                                         {holdingID = rdr.GetGuid(9).ToString()};
                }
            }

            return orderDataModel;
        }

        /// <summary>
        /// Gets the holding by ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        public virtual HoldingDataModel getHolding(string userID, string holdingID)
        {
            SqlParameter parm1 = new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier)
                                     {Value = Guid.Parse(holdingID)};

            HoldingDataModel holding = null;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_HOLDING, parm1))
            {
                if (rdr.Read())
                {
                    holding = new HoldingDataModel(rdr.GetGuid(0).ToString(), rdr.GetDouble(1),
                                                   rdr.GetDecimal(2), rdr.GetDateTime(3),
                                                   rdr.GetString(4), rdr.GetGuid(5).ToString(), 0);
                }
            }

            return holding;
        }

        /// <summary>
        /// Creates a new holding.
        /// </summary>
        /// <param name="order">The order.</param>
        public virtual string createHolding(OrderDataModel order)
        {
            SqlParameter orderParm = new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier)
                                         {Value = Guid.Parse(order.orderID)};

            Guid holdingId = Guid.NewGuid();
            Guid accountId;

            using (SqlDataReader rdr = _helper.ExecuteReaderSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_GET_ACCOUNTID_ORDER, orderParm))
            {
                rdr.Read();
                accountId = rdr.GetGuid(0);
            }

            SqlParameter[] holdingParms = GetCreateHoldingParameters();
            holdingParms[0].Value = order.price;
            holdingParms[1].Value = (float)order.quantity;
            holdingParms[2].Value = order.openDate;
            holdingParms[3].Value = accountId;
            holdingParms[4].Value = order.symbol;
            holdingParms[5].Value = order.userID.GetSignedMurmur2HashCode();
            holdingParms[6].Value = holdingId;

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_INSERT_HOLDING, holdingParms);
            
            return holdingId.ToString();
        }

        /// <summary>
        /// Updates the holding quantity (on sellEnhanced).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingid">The holdingid.</param>
        /// <param name="quantity">The quantity.</param>
        public virtual void updateHolding(string userID, string holdingid, double quantity)
        {
            SqlParameter[] holdingParms2 = GetUpdateHoldingParameters();
            holdingParms2[0].Value = Guid.Parse(holdingid);
            holdingParms2[1].Value = quantity;

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_UPDATE_HOLDING, holdingParms2);
        }

        /// <summary>
        /// Deletes the holding (on sell/sellEnhanced).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingid">The holdingid.</param>
        public virtual void deleteHolding(string userID, string holdingid)
        {
            SqlParameter[] holdingParms2 = { new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier) };
            holdingParms2[0].Value = Guid.Parse(holdingid);

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_DELETE_HOLDING, holdingParms2);
        }

        /// <summary>
        /// Updates the order.
        /// </summary>
        /// <param name="order">The order.</param>
        public virtual void updateOrder(OrderDataModel order)
        {
            SqlParameter[] orderparms = GetUpdateOrderParameters();
            orderparms[0].Value = order.quantity;
            orderparms[1].Value = Guid.Parse(order.orderID);

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_UPDATE_ORDER, orderparms);
        }

        /// <summary>
        /// Closes the order.
        /// </summary>
        /// <param name="order">The order.</param>
        public virtual void closeOrder(OrderDataModel order)
        {
            SqlParameter[] closeorderparm = GetCloseOrdersParameters();
            closeorderparm[0].Value = StockTraderUtility.ORDER_STATUS_CLOSED;
            closeorderparm[1].Value = order.orderType.Equals(StockTraderUtility.ORDER_TYPE_SELL)
                                          ? DBNull.Value
                                          : (object)Guid.Parse(order.holdingID);
            closeorderparm[2].Value = order.price;
            closeorderparm[3].Value = Guid.Parse(order.orderID);

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_CLOSE_ORDER, closeorderparm);   
        }

        /// <summary>
        /// Create a new customer DAL object.
        /// </summary>
        protected virtual ICustomer CreateCustomer(SqlConnection internalConnection, SqlTransaction internalAdoTransaction)
        {
            return new Customer(internalConnection, internalAdoTransaction);
        }

        /// <summary>
        /// Create a new market summary DAL object.
        /// </summary>
        protected virtual IMarketSummary CreateMarketSummary(SqlConnection internalConnection, SqlTransaction internalAdoTransaction)
        {
            return new MarketSummary(internalConnection, internalAdoTransaction);
        }

        /// <summary>
        /// Gets the create order parameters.
        /// </summary>
        private static SqlParameter[] GetCreateOrderParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_INSERT_ORDER);
            
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[]
                            {
                                new SqlParameter(PARM_ORDERFEE, SqlDbType.Decimal, 14),
                                new SqlParameter(PARM_PRICE, SqlDbType.Decimal, 14),
                                new SqlParameter(PARM_SYMBOL, SqlDbType.VarChar, 20),
                                new SqlParameter(PARM_QUANTITY, SqlDbType.Float),
                                new SqlParameter(PARM_ORDERTYPE, SqlDbType.VarChar, 5),
                                new SqlParameter(PARM_ACCOUNTID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_USERIDHASH, SqlDbType.Int)
                            };
                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_INSERT_ORDER, parms);
            }

            return parms;
        }

        /// <summary>
        /// Gets the update order parameters.
        /// </summary>
        private static SqlParameter[] GetUpdateOrderParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_UPDATE_ORDER);
            
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[]
                            {
                                new SqlParameter(PARM_QUANTITY, SqlDbType.Float),
                                new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier)
                            };

                SQLHelper.CacheParameters(SQL_UPDATE_ORDER, parms);
            }

            return parms;
        }

        /// <summary>
        /// Gets the create holding parameters.
        /// </summary>
        private static SqlParameter[] GetCreateHoldingParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_INSERT_HOLDING);
            
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                           new SqlParameter(PARM_PURCHASEPRICE, SqlDbType.Decimal, 14),
                           new SqlParameter(PARM_QUANTITY, SqlDbType.Float),
                           new SqlParameter(PARM_PURCHASEDATE, SqlDbType.DateTime),
                       	   new SqlParameter(PARM_ACCOUNTID, SqlDbType.UniqueIdentifier),
                           new SqlParameter(PARM_SYMBOL, SqlDbType.VarChar, 20),
                           new SqlParameter(PARM_USERIDHASH, SqlDbType.Int),
                           new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier) };

                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_INSERT_HOLDING, parms);
            }

            return parms;
        }

        /// <summary>
        /// Gets the update holding parameters.
        /// </summary>
        private static SqlParameter[] GetUpdateHoldingParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_UPDATE_HOLDING);
            
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[]
                            {
                                new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_QUANTITY, SqlDbType.Float)
                            };

                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_UPDATE_HOLDING, parms);
            }

            return parms;
        }

        /// <summary>
        /// Gets the close orders parameters.
        /// </summary>
        private static SqlParameter[] GetCloseOrdersParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_CLOSE_ORDER);
            
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                        new SqlParameter(PARM_ORDERSTATUS, SqlDbType.VarChar,10),
                        new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier),
                        new SqlParameter(PARM_PRICE, SqlDbType.Decimal, 14),
                        new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier)};

                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_CLOSE_ORDER, parms);
            }

            return parms;
        }
    }
}
