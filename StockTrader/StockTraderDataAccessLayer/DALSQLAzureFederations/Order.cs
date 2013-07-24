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

using System.Data.SqlClient;
using System.Transactions;
using Trade.BusinessServiceDataContract;
using Trade.IDAL;
using IsolationLevel = System.Data.IsolationLevel;

namespace Trade.DALSQLAzure.Federations
{
    /// <summary>
    /// Order implementation for SQL Azure with Federations.
    /// </summary>
    public class Order : DALSQLAzure.Order
    {
        private readonly SqlAzureFederationsHelper _helper;
        private bool _inAdoTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        public Order()
            : this(null, null, new SqlAzureFederationsHelper())
        {
        }

        /// <summary>
        /// Constructor for internal DAL-DAL calls to use an existing DB connection.
        /// </summary>
        internal Order(SqlConnection conn, SqlTransaction trans, SqlAzureFederationsHelper helper)
            : base(conn, trans, helper)
        {
            _helper = helper;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public override void BeginADOTransaction()
        {
            _inAdoTransaction = true;
        }

        /// <summary>
        /// Used only when explicitly using ADO.NET transactions from the BSL.
        /// </summary>
        public override void RollBackTransaction()
        {
            _inAdoTransaction = false;
            base.RollBackTransaction();
        }

        /// <summary>
        /// Used only when explicitly using ADO.NET transactions from the BSL.
        /// </summary>
        public override void CommitADOTransaction()
        {
            _inAdoTransaction = false;
            base.CommitADOTransaction();
        }

        /// <summary>
        /// Opens the connection using the specified connnection string.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        public override void Open(string connString)
        {
            base.Open(connString);
            _helper.Initialize(FederationFactory.CreateAccountFederation(_internalConnection));
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="orderType">Type of the order.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="holdingID">The holding ID.</param>
        public override OrderDataModel createOrder(string userID, string symbol, string orderType, double quantity, string holdingID)
        {
            Select(userID, true);
            return base.createOrder(userID, symbol, orderType, quantity, holdingID);
        }

        /// <summary>
        /// Gets the order by ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="orderID">The order ID.</param>
        public override OrderDataModel getOrder(string userID, string orderID)
        {
            Select(userID, true);
            return base.getOrder(userID, orderID);
        }

        /// <summary>
        /// Gets the holding by ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        public override HoldingDataModel getHolding(string userID, string holdingID)
        {
            Select(userID, true);
            return base.getHolding(userID, holdingID);
        }

        /// <summary>
        /// Creates a new holding.
        /// </summary>
        /// <param name="order">The order.</param>
        public override string createHolding(OrderDataModel order)
        {
            Select(order.userID, true);
            return base.createHolding(order);
        }

        /// <summary>
        /// Updates the holding quantity (on sellEnhanced).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingid">The holdingid.</param>
        /// <param name="quantity">The quantity.</param>
        public override void updateHolding(string userID, string holdingid, double quantity)
        {
            Select(userID, true);
            base.updateHolding(userID, holdingid, quantity);
        }

        /// <summary>
        /// Deletes the holding (on sell/sellEnhanced).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingid">The holdingid.</param>
        public override void deleteHolding(string userID, string holdingid)
        {
            Select(userID, true);
            base.deleteHolding(userID, holdingid);
        }

        /// <summary>
        /// Updates the order.
        /// </summary>
        /// <param name="order">The order.</param>
        public override void updateOrder(OrderDataModel order)
        {
            Select(order.userID, true);
            base.updateOrder(order);
        }

        /// <summary>
        /// Closes the order.
        /// </summary>
        /// <param name="order">The order.</param>
        public override void closeOrder(OrderDataModel order)
        {
            Select(order.userID, true);
            base.closeOrder(order);
        }

        /// <summary>
        /// Create a new customer DAL object.
        /// </summary>
        protected override ICustomer CreateCustomer(SqlConnection internalConnection, SqlTransaction internalAdoTransaction)
        {
            return new Customer(internalConnection, internalAdoTransaction, _helper);
        }

        /// <summary>
        /// Create a new market summary DAL object.
        /// </summary>
        protected override IMarketSummary CreateMarketSummary(SqlConnection internalConnection, SqlTransaction internalAdoTransaction)
        {
            return new MarketSummary(internalConnection, internalAdoTransaction, _helper);
        }

        /// <summary>
        /// Selects the federation.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="isFiltered">if set to <c>true</c> [is filtered].</param>
        private void Select(string userId, bool isFiltered)
        {
            bool changed = userId == null
                               ? _helper.Reset()
                               : _helper.Select(userId.GetSignedMurmur2HashCode(), isFiltered);

            if (changed)
            {
                // Connection will already be open and cannot be enlisted in a transaction when issuing 
                // a USE FEDERATION command. This will explicitly enlist the connection in a transaction.
                if (Transaction.Current != null)
                {
                    _internalConnection.EnlistTransaction(Transaction.Current);
                }
                else if (_inAdoTransaction)
                {
                    _internalADOTransaction = _internalConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                }
            }
        }
    }
}
