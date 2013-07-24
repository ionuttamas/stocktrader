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

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using Trade.BusinessServiceDataContract;
using IsolationLevel = System.Data.IsolationLevel;

namespace Trade.DALSQLAzure.Federations
{
    /// <summary>
    /// Customer implementation for SQL Azure with Federations.
    /// </summary>
    public class Customer : DALSQLAzure.Customer
    {
        private readonly SqlAzureFederationsHelper _helper;
        private bool _inAdoTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        public Customer()
            : this(null, null, new SqlAzureFederationsHelper())
        {
        }

        /// <summary>
        /// Constructor for internal DAL-DAL calls to use an existing DB connection.
        /// </summary>
        internal Customer(SqlConnection conn, SqlTransaction trans, SqlAzureFederationsHelper helper)
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
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public override void RollBackTransaction()
        {
            _inAdoTransaction = false;
            base.RollBackTransaction();
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
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
        /// Logs in the specified userid.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="password">The password.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public override AccountDataModel login(string userid, string password, bool useSaltedHash)
        {
            Select(userid, true);
            return base.login(userid, password, useSaltedHash);
        }

        /// <summary>
        /// Logs the out user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override void logOutUser(string userID)
        {
            Select(userID, true);
            base.logOutUser(userID);
        }

        /// <summary>
        /// Validates the specified user/pass combo.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="password">The password.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public override bool validate(string userid, string password, bool useSaltedHash)
        {
            Select(userid, true);
            return base.validate(userid, password, useSaltedHash);
        }

        /// <summary>
        /// Gets the account profile data.
        /// </summary>
        /// <param name="userid">The userid.</param>
        public override AccountProfileDataModel getAccountProfileData(string userid)
        {
            Select(userid, true);
            return base.getAccountProfileData(userid);
        }

        /// <summary>
        /// Gets the closed orders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public override List<OrderDataModel> getClosedOrders(string userId)
        {
            Select(userId, true);
            return base.getClosedOrders(userId);
        }

        /// <summary>
        /// Gets the account by user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override AccountDataModel getCustomerByUserID(string userID)
        {
            Select(userID, true);
            return base.getCustomerByUserID(userID);
        }

        /// <summary>
        /// Gets the holding by user ID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="holdingid">The holdingid.</param>
        public override HoldingDataModel getHolding(string userid, string holdingid)
        {
            Select(userid, true);
            return base.getHolding(userid, holdingid);
        }

        /// <summary>
        /// Gets the holding (for update).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="orderID">The order ID.</param>
        public override HoldingDataModel getHoldingForUpdate(string userID, string orderID)
        {
            Select(userID, true);
            return base.getHoldingForUpdate(userID, orderID);
        }

        /// <summary>
        /// Gets all holdings for a user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override List<HoldingDataModel> getHoldings(string userID)
        {
            Select(userID, true);
            return base.getHoldings(userID);
        }

        /// <summary>
        /// Gets the orders for a user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="top">if set to <c>true</c> [top].</param>
        /// <param name="maxTop">The max top.</param>
        /// <param name="maxDefault">The max default.</param>
        public override List<OrderDataModel> getOrders(string userID, bool top, int maxTop, int maxDefault)
        {
            Select(userID, true);
            return base.getOrders(userID, top, maxTop, maxDefault);
        }

        /// <summary>
        /// Inserts a new account.
        /// </summary>
        /// <param name="customer">The account details.</param>
        public override void insertAccount(AccountDataModel customer)
        {
            Select(customer.profileID, false);
            base.insertAccount(customer);
        }

        /// <summary>
        /// Inserts a new account profile.
        /// </summary>
        /// <param name="customerprofile">The account profile.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public override void insertAccountProfile(AccountProfileDataModel customerprofile, bool useSaltedHash)
        {
            Select(customerprofile.userID, false);
            base.insertAccountProfile(customerprofile, useSaltedHash);
        }

        /// <summary>
        /// Updates the specified customerprofile.
        /// </summary>
        /// <param name="customerprofile">The account profile.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public override AccountProfileDataModel update(AccountProfileDataModel customerprofile, bool useSaltedHash)
        {
            Select(customerprofile.userID, true);
            return base.update(customerprofile, useSaltedHash);
        }

        /// <summary>
        /// Updates the account balance.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="total">The total value to update the balance by.</param>
        public override void updateAccountBalance(string userID, decimal total)
        {
            Select(userID, true);
            base.updateAccountBalance(userID, total);
        }

        /// <summary>
        /// Gets the index of the news topic for a given device.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="deviceID">The device ID.</param>
        public override int? getAccountDeviceTopicIndex(string userID, string deviceID)
        {
            Select(userID, true);
            return base.getAccountDeviceTopicIndex(userID, deviceID);
        }

        /// <summary>
        /// Gets the new index of the account device news topic. This is used when creating a new device subscription.
        /// </summary>
        public override int getNewAccountDeviceTopicIndex()
        {
            Reset();
            return base.getNewAccountDeviceTopicIndex();
        }

        /// <summary>
        /// Inserts the account device.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="deviceID">The device ID.</param>
        /// <param name="topicIndex">Index of the topic.</param>
        public override void insertAccountDeviceTopicIndex(string userID, string deviceID, int topicIndex)
        {
            Select(userID, false);
            base.insertAccountDeviceTopicIndex(userID, deviceID, topicIndex);
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

        /// <summary>
        /// Resets the federation.
        /// </summary>
        private void Reset()
        {
            Select(null, false);
        }
    }
}
