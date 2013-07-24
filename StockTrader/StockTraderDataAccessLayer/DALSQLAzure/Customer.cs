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


//===============================================================================================
// Customer is part of the SQLAzure DAL for StockTrader.  This is called from the
// BSL to execute commands against the database.  It is constructed to use one SqlConnection per
// instance.  Hence, BSLs that use this DAL should always be instanced properly.
// The DAL will work with both ADO.NET and System.Transactions or ServiceComponents/Enterprise
// Services attributed transactions [autocomplete]. When using ADO.NET transactions,
// The BSL will control the transaction boundaries with calls to dal.BeginTransaction(); 
// dal.CommitTransaction(); dal.RollbackTransaction().
//===============================================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ConfigService.DALSQLHelper;
using Trade.BusinessServiceDataContract;
using Trade.IDAL;
using Trade.Utility;

namespace Trade.DALSQLAzure
{
    /// <summary>
    /// Customer implementation for SQL Azure.
    /// </summary>
    public class Customer : ICustomer
    {
        private const string SQL_SELECT_HOLDINGS = "SET NOCOUNT ON; SELECT TOP 250 HOLDING.HOLDINGID, HOLDING.QUANTITY, HOLDING.PURCHASEPRICE, HOLDING.PURCHASEDATE, HOLDING.QUOTE_SYMBOL, HOLDING.ACCOUNT_ACCOUNTID from dbo.holding WHERE HOLDING.ACCOUNT_ACCOUNTID = (SELECT ACCOUNTID FROM ACCOUNT WHERE PROFILE_USERID = @UserId) ORDER BY HOLDING.HOLDINGID DESC";
        private const string SQL_SELECT_HOLDING_LOCK = @"SET NOCOUNT ON; SELECT dbo.HOLDING.HOLDINGID, HOLDING.ACCOUNT_ACCOUNTID, HOLDING.QUANTITY, HOLDING.PURCHASEPRICE, HOLDING.PURCHASEDATE, HOLDING.QUOTE_SYMBOL, Quote.Price FROM dbo.HOLDING WITH (ROWLOCK) Inner Join Quote WITH (ROWLOCK) ON Holding.Quote_Symbol=Quote.Symbol INNER JOIN ORDERS WITH (ROWLOCK) ON HOLDING.HOLDINGID = ORDERS.HOLDING_HOLDINGID
                                                       WHERE (ORDERS.ORDERID = @OrderId)";
        private const string SQL_SELECT_HOLDING_NOLOCK = "SET NOCOUNT ON; SELECT HOLDING.ACCOUNT_ACCOUNTID, HOLDING.QUANTITY, HOLDING.PURCHASEPRICE, HOLDING.PURCHASEDATE, HOLDING.QUOTE_SYMBOL FROM HOLDING WITH(NOLOCK) WHERE HOLDING.HOLDINGID=@holdingId AND HOLDING.ACCOUNT_ACCOUNTID = (SELECT ACCOUNTID FROM dbo.ACCOUNT WITH (NOLOCK) WHERE PROFILE_USERID = @UserId)";
        private const string SQL_SELECT_GET_CUSTOMER_BYUSERID = "SET NOCOUNT ON; SELECT account.ACCOUNTID, account.PROFILE_USERID, account.CREATIONDATE, account.OPENBALANCE, account.LOGOUTCOUNT, account.BALANCE, account.LASTLOGIN, account.LOGINCOUNT FROM account WHERE account.PROFILE_USERID = @UserId";
        private const string SQL_SELECT_CUSTOMERPROFILE_BYUSERID = "SET NOCOUNT ON; SELECT accountprofile.USERID,accountprofile.SALT, accountprofile.PASSWORD, accountprofile.FULLNAME, accountprofile.ADDRESS, accountprofile.EMAIL, accountprofile.CREDITCARD FROM dbo.accountprofile WITH (NOLOCK) WHERE accountprofile.USERID = @UserId";
        private const string SQL_SELECT_UPDATE_CUSTOMER_LOGIN = "SET NOCOUNT ON; UPDATE dbo.account WITH (ROWLOCK) SET LOGINCOUNT = (LOGINCOUNT + 1), LASTLOGIN = CURRENT_TIMESTAMP where PROFILE_USERID= @UserId; SELECT account.ACCOUNTID, account.CREATIONDATE, account.OPENBALANCE, account.LOGOUTCOUNT, account.BALANCE, account.LASTLOGIN, account.LOGINCOUNT FROM dbo.account WITH (ROWLOCK) WHERE account.PROFILE_USERID = @UserId";
        private const string SQL_UPDATE_LOGOUT = "SET NOCOUNT ON; UPDATE dbo.account WITH (ROWLOCK) SET LOGOUTCOUNT = (LOGOUTCOUNT + 1) where PROFILE_USERID= @UserId";
        private const string SQL_UPDATE_ACCOUNTPROFILE = "SET NOCOUNT ON; UPDATE dbo.accountprofile WITH (ROWLOCK) SET ADDRESS=@Address, SALT=@Salt, PASSWORD=@Password, EMAIL=@Email, CREDITCARD = @CreditCard, FULLNAME=@FullName WHERE USERID= @UserId";
        private const string SQL_SELECT_CLOSED_ORDERS = "SET NOCOUNT ON; SELECT ORDERID, ORDERTYPE, ORDERSTATUS, COMPLETIONDATE, OPENDATE, QUANTITY, PRICE, ORDERFEE, QUOTE_SYMBOL FROM dbo.orders WHERE ACCOUNT_ACCOUNTID = (select accountid from dbo.account WITH(NOLOCK) where profile_userid =@UserId) AND ORDERSTATUS = 'closed'";
        private const string SQL_UPDATE_CLOSED_ORDERS = "SET NOCOUNT ON; UPDATE dbo.orders WITH (ROWLOCK) SET ORDERSTATUS = 'completed' where ORDERSTATUS = 'closed' AND ACCOUNT_ACCOUNTID = (select accountid from dbo.account WITH (NOLOCK) where profile_userid =@UserId)";
        private const string SQL_SELECT_ORDERS_BY_OPENDATE = " o.ORDERID, o.ORDERTYPE, o.ORDERSTATUS, o.OPENDATE, o.COMPLETIONDATE, o.QUANTITY, o.PRICE, o.ORDERFEE, o.QUOTE_SYMBOL from dbo.orders o where o.account_accountid = (select a.accountid from dbo.account a WITH (NOLOCK)  where a.profile_userid = @UserId) ORDER BY o.OPENDATE DESC";
        private const string SQL_INSERT_ACCOUNTPROFILE = "SET NOCOUNT ON; INSERT INTO dbo.accountprofile VALUES (@Address, @Password, @UserId, @UserIdHash, @Email, @CreditCard, @FullName, @Salt)";
        private const string SQL_INSERT_ACCOUNT = "SET NOCOUNT ON; INSERT INTO dbo.account (CREATIONDATE, OPENBALANCE, LOGOUTCOUNT, BALANCE, ACCOUNTID, LASTLOGIN, LOGINCOUNT, PROFILE_USERID, PROFILE_USERIDHASH) VALUES (GetDate(), @OpenBalance, @LogoutCount, @Balance, newid(), @LastLogin, @LoginCount, @UserId, @UserIdHash);";
        private const string SQL_DEBIT_ACCOUNT = "SET NOCOUNT ON; UPDATE dbo.ACCOUNT WITH (ROWLOCK) SET BALANCE=(BALANCE-@Debit) WHERE PROFILE_USERID=@UserId";
        private const string SQL_SELECT_ACCOUNTDEVICE = "SET NOCOUNT ON; SELECT TOPIC FROM dbo.accountdevice WHERE DEVICEID = @DeviceId AND PROFILE_USERID = @UserId";
        private const string SQL_SELECT_NEW_NEWSTOPIC = "SET NOCOUNT ON; BEGIN TRANSACTION; DECLARE @TOPIC INT; SELECT @TOPIC = MIN(TOPIC) FROM NEWSTOPICS WHERE SUBSCRIPTIONCOUNT < 2000; IF (@TOPIC IS NULL) BEGIN SELECT @TOPIC = (COALESCE(MAX(TOPIC), -1) + 1) FROM NEWSTOPICS; INSERT INTO NEWSTOPICS VALUES(@TOPIC, 1); END; ELSE UPDATE NEWSTOPICS SET SUBSCRIPTIONCOUNT = (SUBSCRIPTIONCOUNT + 1) WHERE TOPIC = @TOPIC; SELECT @TOPIC; COMMIT TRANSACTION;";
        private const string SQL_INSERT_ACCOUNTDEVICE = "SET NOCOUNT ON; INSERT INTO dbo.accountdevice VALUES(@DeviceId, @Topic, @UserId, @UserIdHash)";
        
        //Parameters
        private const string PARM_USERID = "@UserId";
        private const string PARM_HOLDINGID = "@holdingId";
        private const string PARM_ORDERID = "@OrderId";
        private const string PARM_PASSWORD = "@Password";
        private const string PARM_FULLNAME = "@FullName";
        private const string PARM_ADDRESS = "@Address";
        private const string PARM_EMAIL = "@Email";
        private const string PARM_CREDITCARD = "@CreditCard";
        private const string PARM_OPENBALANCE = "@OpenBalance";
        private const string PARM_LOGOUTCOUNT = "@LogoutCount";
        private const string PARM_BALANCE = "@Balance";
        private const string PARM_LASTLOGIN = "@LastLogin";
        private const string PARM_LOGINCOUNT = "@LoginCount";
        private const string PARM_DEBIT = "@Debit";
        private const string PARM_SALT = "@Salt";
        private const string PARM_DEVICEID = "@DeviceId";
        private const string PARM_TOPIC = "@Topic";
        private const string PARM_USERIDHASH = "@UserIdHash";

        protected SqlConnection _internalConnection;
        protected SqlTransaction _internalADOTransaction;
        private readonly SqlAzureHelper _helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        public Customer()
            : this (null, null)
        {
        }

        /// <summary>
        /// Constructor for internal DAL-DAL calls to use an existing DB connection.
        /// </summary>
        protected internal Customer(SqlConnection conn, SqlTransaction trans)
            : this (conn, trans, new SqlAzureHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        protected Customer(SqlConnection conn, SqlTransaction trans, SqlAzureHelper helper)
        {
            _internalConnection = conn;
            _internalADOTransaction = trans;
            _helper = helper;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// This will be completely ignored when null, and not attached to a cmd object
        /// In SQLHelper unless it has been initialized explicitly in the BSL with a
        /// dal.BeginADOTransaction().  See app config setting in web.config and 
        /// Trade.BusinessServiceHost.exe.config "Use System.Transactions Globally" which determines
        /// whether user wants to run with ADO transactions or System.Transactions.  The DAL itself
        /// is built to be completely agnostic and will work with either.
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
        /// Used only when doing ADO.NET transactions.
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
        /// Used only when doing ADO.NET transactions.
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
            const string sql = "SELECT 0";
            _helper.ExecuteScalarNoParm(_internalConnection, _internalADOTransaction, CommandType.Text, sql);
        }

        /// <summary>
        /// Logs in the specified userid.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="password">The password.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public virtual AccountDataModel login(string userid, string password, bool useSaltedHash)
        {
            var valid = validate(userid, password, useSaltedHash);

            // Update the user login count and retrieve the data model
            AccountDataModel customer = null;
            if (valid)
            {
                using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_UPDATE_CUSTOMER_LOGIN, GetUserIdParameter(userid)))
                {
                    if (rdr.Read())
                    {
                        customer = new AccountDataModel(rdr.GetGuid(0).ToString(), userid, rdr.GetDateTime(1), rdr.GetDecimal(2), rdr.GetInt32(3), rdr.GetDecimal(4), rdr.GetDateTime(5), rdr.GetInt32(6) + 1);
                    }
                }
            }

            return customer;
        }

        /// <summary>
        /// Logs the out user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual void logOutUser(string userID)
        {
            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_UPDATE_LOGOUT, GetUserIdParameter(userID));
        }

        /// <summary>
        /// Validates the specified user/pass combo.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="password">The password.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public virtual bool validate(string userid, string password, bool useSaltedHash)
        {
            // Validate the user id/password combination
            bool valid = false;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_CUSTOMERPROFILE_BYUSERID, GetUserIdParameter(userid)))
            {
                if (rdr.Read())
                {
                    string salt = rdr.GetString(1);
                    string userPassword = rdr.GetString(2);
                    valid = useSaltedHash ? SaltedHash.Create(salt, userPassword).Verify(password) : password.Equals(userPassword);
                }
            }

            return valid;
        }

        /// <summary>
        /// Gets the account profile data.
        /// </summary>
        /// <param name="userid">The userid.</param>
        public virtual AccountProfileDataModel getAccountProfileData(string userid)
        {
            AccountProfileDataModel customerProfile = null;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_CUSTOMERPROFILE_BYUSERID, GetUserIdParameter(userid)))
            {
                if (rdr.Read())
                {
                    customerProfile = new AccountProfileDataModel(userid, rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5), rdr.GetString(6));
                }
            }
            
            return customerProfile;
        }

        /// <summary>
        /// Gets the closed orders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public virtual List<OrderDataModel> getClosedOrders(string userId)
        {
            List<OrderDataModel> closedorders = new List<OrderDataModel>();
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_CLOSED_ORDERS, GetUserIdParameter(userId)))
            {
                DateTime completionDate = DateTime.MinValue;
                while (rdr.Read())
                {
                    DateTime openDate = rdr.GetDateTime(4);
                    
                    try
                    {
                        completionDate = rdr.GetDateTime(3);
                    }
                    catch (Exception e) { if (e.Message.Equals("Data is Null. This method or property cannot be called on Null values.")) completionDate = DateTime.MinValue; }
                    
                    OrderDataModel order = new OrderDataModel(rdr.GetGuid(0).ToString(), rdr.GetString(1), rdr.GetString(2), openDate, completionDate, rdr.GetDouble(5), rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetString(8), userId);
                    order.orderStatus = StockTraderUtility.ORDER_STATUS_COMPLETED;
                    closedorders.Add(order);
                }
            }

            if (closedorders.Count > 0)
            {
                _helper.ExecuteNonQuerySingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_UPDATE_CLOSED_ORDERS, GetUserIdParameter(userId));
            }

            return closedorders;
        }

        /// <summary>
        /// Gets the account by user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual AccountDataModel getCustomerByUserID(string userID)
        {
            AccountDataModel customer = null;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_GET_CUSTOMER_BYUSERID, GetUserIdParameter(userID)))
            {
                if (rdr.Read())
                {
                    customer = new AccountDataModel(rdr.GetGuid(0).ToString(), userID, rdr.GetDateTime(2), rdr.GetDecimal(3), rdr.GetInt32(4), rdr.GetDecimal(5), rdr.GetDateTime(6), rdr.GetInt32(7));
                }
            }

            return customer;
        }

        /// <summary>
        /// Gets the holding by user ID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="holdingid">The holdingid.</param>
        public virtual HoldingDataModel getHolding(string userid, string holdingid)
        {
            SqlParameter[] holdingidparms = new[]{new SqlParameter(PARM_HOLDINGID, SqlDbType.UniqueIdentifier), 
                                                GetUserIdParameter(userid)};
            holdingidparms[0].Value = Guid.Parse(holdingid);
            
            HoldingDataModel holding = null;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRow(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_HOLDING_NOLOCK, holdingidparms))
            {
                if (rdr.Read())
                {
                    holding = new HoldingDataModel(holdingid, rdr.GetGuid(0).ToString(), rdr.GetDouble(1), rdr.GetDecimal(2), rdr.GetDateTime(3), rdr.GetString(4), 0);
                }
            }

            return holding;
        }

        /// <summary>
        /// Gets the holding (for update).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="orderID">The order ID.</param>
        public virtual HoldingDataModel getHoldingForUpdate(string userID, string orderID)
        {
            SqlParameter orderIDparm = new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier);
            orderIDparm.Value = Guid.Parse(orderID);

            HoldingDataModel holding = null;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_HOLDING_LOCK, orderIDparm))
            {
                if (rdr.Read())
                {
                    holding = new HoldingDataModel(rdr.GetGuid(0).ToString(), rdr.GetGuid(1).ToString(), rdr.GetDouble(2), rdr.GetDecimal(3), rdr.GetDateTime(4), rdr.GetString(5), 0);
                }
            }

            return holding;
        }

        /// <summary>
        /// Gets all holdings for a user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public virtual List<HoldingDataModel> getHoldings(string userID)
        {
            List<HoldingDataModel> holdings = new List<HoldingDataModel>();
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_HOLDINGS, 
                GetUserIdParameter(userID)))
            {
                while (rdr.Read())
                {
                    HoldingDataModel holding = new HoldingDataModel(rdr.GetGuid(0).ToString(), rdr.GetDouble(1),
                                                                    rdr.GetDecimal(2), rdr.GetDateTime(3),
                                                                    rdr.GetString(4), rdr.GetGuid(5).ToString(), 0);
                    holdings.Add(holding);
                }
            }

            return holdings;
        }

        /// <summary>
        /// Gets the orders for a user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="top">if set to <c>true</c> [top].</param>
        /// <param name="maxTop">The max top.</param>
        /// <param name="maxDefault">The max default.</param>
        public virtual List<OrderDataModel> getOrders(string userID, bool top, int maxTop, int maxDefault)
        {
            string commandText;
            if (top)
            {
                commandText = "Select Top " + maxTop + SQL_SELECT_ORDERS_BY_OPENDATE;
            }
            else
            {
                commandText = "Select Top " + maxDefault + SQL_SELECT_ORDERS_BY_OPENDATE;
            }

            List<OrderDataModel> orders = new List<OrderDataModel>();
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, commandText,
                GetUserIdParameter(userID)))
            {
                while (rdr.Read())
                {
                    object completionDate;
                    
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

                    OrderDataModel order = new OrderDataModel(rdr.GetGuid(0).ToString(), rdr.GetString(1), rdr.GetString(2),
                                                              rdr.GetDateTime(3), (DateTime) completionDate, rdr.GetDouble(5),
                                                              rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetString(8), userID);
                    orders.Add(order);
                }
            }

            return orders;
        }

        /// <summary>
        /// Inserts a new account.
        /// </summary>
        /// <param name="customer">The account details.</param>
        public virtual void insertAccount(AccountDataModel customer)
        {
            SqlParameter[] accountParms = GetCreateAccountParameters();
            accountParms[0].Value = customer.openBalance;
            accountParms[1].Value = customer.logoutCount;
            accountParms[2].Value = customer.balance;
            accountParms[3].Value = customer.lastLogin;
            accountParms[4].Value = customer.loginCount;
            accountParms[5].Value = customer.profileID;
            accountParms[6].Value = customer.profileID.GetSignedMurmur2HashCode();

            _helper.ExecuteScalar(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_INSERT_ACCOUNT, accountParms);
        }

        /// <summary>
        /// Inserts a new account profile.
        /// </summary>
        /// <param name="customerprofile">The account profile.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public virtual void insertAccountProfile(AccountProfileDataModel customerprofile, bool useSaltedHash)
        {
            SqlParameter[] profileParms = GetCreateAccountProfileParameters();
            string salt = " ";

            if (useSaltedHash)
            {
                SaltedHash sh = SaltedHash.Create(customerprofile.password);
                salt = sh.Salt;
                customerprofile.password = sh.Hash;
            }

            profileParms[0].Value = customerprofile.address;
            profileParms[1].Value = salt;
            profileParms[2].Value = customerprofile.password;
            profileParms[3].Value = customerprofile.userID;
            profileParms[4].Value = customerprofile.email;
            profileParms[5].Value = customerprofile.creditCard;
            profileParms[6].Value = customerprofile.fullName;
            profileParms[7].Value = customerprofile.userID.GetSignedMurmur2HashCode();

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_INSERT_ACCOUNTPROFILE, profileParms);
        }

        /// <summary>
        /// Updates the specified customerprofile.
        /// </summary>
        /// <param name="customerprofile">The account profile.</param>
        /// <param name="useSaltedHash">if set to <c>true</c> [use salted hash].</param>
        public virtual AccountProfileDataModel update(AccountProfileDataModel customerprofile, bool useSaltedHash)
        {
            SqlParameter[] profileParms = GetUpdateAccountProfileParameters();
            string salt = " ";
            
            if (useSaltedHash)
            {
                SaltedHash sh = SaltedHash.Create(customerprofile.password);
                salt = sh.Salt;
                customerprofile.password = sh.Hash;
            }
            
            profileParms[0].Value = customerprofile.address;
            profileParms[1].Value = salt;
            profileParms[2].Value = customerprofile.password;
            profileParms[3].Value = customerprofile.email;
            profileParms[4].Value = customerprofile.creditCard;
            profileParms[5].Value = customerprofile.fullName;
            profileParms[6].Value = customerprofile.userID;

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_UPDATE_ACCOUNTPROFILE, profileParms);
            return customerprofile;
        }

        /// <summary>
        /// Updates the account balance.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="total">The total value to update the balance by.</param>
        public virtual void updateAccountBalance(string userID, decimal total)
        {
            // Get the parameters from the cache
            SqlParameter[] accountParms = GetUpdateAccountBalanceParameters();
            accountParms[0].Value = total;
            accountParms[1].Value = userID;

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_DEBIT_ACCOUNT, accountParms);
        }

        /// <summary>
        /// Gets the index of the news topic for a given device.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="deviceID">The device ID.</param>
        public virtual int? getAccountDeviceTopicIndex(string userID, string deviceID)
        {
            int? topic = null;

            SqlParameter userIdParameter = GetUserIdParameter(userID);
            SqlParameter deviceIdParameter = new SqlParameter(PARM_DEVICEID, SqlDbType.VarChar, 50) { Value = deviceID };

            using (SqlDataReader reader = _helper.ExecuteReader(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_ACCOUNTDEVICE, userIdParameter, deviceIdParameter))
            {
                if (reader.Read())
                {
                    topic = reader.GetInt32(0);
                }
            }

            return topic;
        }

        /// <summary>
        /// Gets the new index of the account device news topic. This is used when creating a new device subscription.
        /// </summary>
        public virtual int getNewAccountDeviceTopicIndex()
        {
            int topic = 0;
            using (SqlDataReader reader = _helper.ExecuteReaderNoParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_NEW_NEWSTOPIC))
            {
                if (reader.Read())
                {
                    topic = reader.GetInt32(0);
                }
            }

            return topic;
        }

        /// <summary>
        /// Inserts the account device.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="deviceID">The device ID.</param>
        /// <param name="topicIndex">Index of the topic.</param>
        public virtual void insertAccountDeviceTopicIndex(string userID, string deviceID, int topicIndex)
        {
            SqlParameter userIdParameter = GetUserIdParameter(userID);
            SqlParameter deviceIdParameter = new SqlParameter(PARM_DEVICEID, SqlDbType.VarChar, 50) { Value = deviceID };
            SqlParameter topicParameter = new SqlParameter(PARM_TOPIC, SqlDbType.Int) { Value = topicIndex };
            SqlParameter userIdHashParameter = new SqlParameter(PARM_USERIDHASH, SqlDbType.Int) { Value = userID.GetSignedMurmur2HashCode() };

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text,
                                    SQL_INSERT_ACCOUNTDEVICE, userIdParameter, deviceIdParameter, topicParameter, userIdHashParameter);
        }

        /// <summary>
        /// Gets the user id parameter.
        /// </summary>
        /// <param name="userId">The user id.</param>
        private static SqlParameter GetUserIdParameter(string userId)
        {
            return new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20) {Value = userId};
        }

        /// <summary>
        /// Gets the update account balance parameters.
        /// </summary>
        private static SqlParameter[] GetUpdateAccountBalanceParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_DEBIT_ACCOUNT);
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                    new SqlParameter(PARM_DEBIT, SqlDbType.Decimal, 14),
                    new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20)};
                
                // Add the parameters to the cached
                SQLHelper.CacheParameters(SQL_DEBIT_ACCOUNT, parms);
            }
            return parms;
        }

        /// <summary>
        /// Gets the create account profile parameters.
        /// </summary>
        private static SqlParameter[] GetCreateAccountProfileParameters()
        {
            // Get the parameters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_INSERT_ACCOUNTPROFILE);
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                      new SqlParameter(PARM_ADDRESS, SqlDbType.VarChar, StockTraderUtility.ADDRESS_MAX_LENGTH),
                      new SqlParameter(PARM_SALT, SqlDbType.VarChar, StockTraderUtility.SALT_MAXLENGTH),
                      new SqlParameter(PARM_PASSWORD, SqlDbType.VarChar, StockTraderUtility.PASSWORD_MAX_LENGTH),
                      new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20),
                      new SqlParameter(PARM_EMAIL, SqlDbType.VarChar, StockTraderUtility.EMAIL_MAX_LENGTH),
                      new SqlParameter(PARM_CREDITCARD, SqlDbType.VarChar, StockTraderUtility.CREDITCARD_MAX_LENGTH),
                      new SqlParameter(PARM_FULLNAME, SqlDbType.VarChar, StockTraderUtility.FULLNAME_MAX_LENGTH),
                      new SqlParameter(PARM_USERIDHASH, SqlDbType.Int) };

                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_INSERT_ACCOUNTPROFILE, parms);
            }
            return parms;
        }

        /// <summary>
        /// Gets the create account parameters.
        /// </summary>
        private static SqlParameter[] GetCreateAccountParameters()
        {
            // Get the parameters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_INSERT_ACCOUNT);
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                      new SqlParameter(PARM_OPENBALANCE, SqlDbType.Decimal),
                      new SqlParameter(PARM_LOGOUTCOUNT, SqlDbType.Int),
                      new SqlParameter(PARM_BALANCE, SqlDbType.Decimal),
                      new SqlParameter(PARM_LASTLOGIN, SqlDbType.DateTime),
                      new SqlParameter(PARM_LOGINCOUNT, SqlDbType.Int),
                      new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20),
                      new SqlParameter(PARM_USERIDHASH, SqlDbType.Int)};

                // Add the parameters to the cached
                SQLHelper.CacheParameters(SQL_INSERT_ACCOUNT, parms);
            }
            return parms;
        }

        /// <summary>
        /// Gets the update account profile parameters.
        /// </summary>
        private static SqlParameter[] GetUpdateAccountProfileParameters()
        {
            // Get the parameters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_UPDATE_ACCOUNTPROFILE);
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                      new SqlParameter(PARM_ADDRESS, SqlDbType.VarChar, StockTraderUtility.ADDRESS_MAX_LENGTH),
                      new SqlParameter(PARM_SALT, SqlDbType.VarChar, StockTraderUtility.PASSWORD_MAX_LENGTH),
                      new SqlParameter(PARM_PASSWORD, SqlDbType.VarChar, StockTraderUtility.PASSWORD_MAX_LENGTH),
                      new SqlParameter(PARM_EMAIL, SqlDbType.VarChar, StockTraderUtility.EMAIL_MAX_LENGTH),
                      new SqlParameter(PARM_CREDITCARD, SqlDbType.VarChar, StockTraderUtility.CREDITCARD_MAX_LENGTH),
                      new SqlParameter(PARM_FULLNAME, SqlDbType.VarChar, StockTraderUtility.FULLNAME_MAX_LENGTH),
                      new SqlParameter(PARM_USERID, SqlDbType.VarChar, 20)};

                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_UPDATE_ACCOUNTPROFILE, parms);
            }
            return parms;
        }
    }
}
