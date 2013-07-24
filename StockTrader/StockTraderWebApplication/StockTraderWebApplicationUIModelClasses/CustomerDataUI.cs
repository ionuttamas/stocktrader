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

namespace Trade.StockTraderWebApplicationModelClasses
{

    /// <summary>
    /// Model class for displaying account data in a web page.
    /// </summary>
    [Serializable]
    public sealed class AccountDataUI
    {
        private string _accountID; 		    
        private DateTime _creationDate; 	
        private string _userId;
        private decimal _openBalance;
        private int _logoutCount;
        private decimal _balance;
        private DateTime _lastLogin;
        private int _loginCount;
            
        public AccountDataUI()
        {
        }

        public AccountDataUI(
            string accountID, 
            string userid,
            DateTime creationDate,
            decimal openBalance,
            int logoutCount,
            decimal balance,
            DateTime lastLogin,
            int loginCount)
        {
            this._accountID = accountID;
            this._creationDate = creationDate;
            this._userId = userid;
            this._openBalance = openBalance;
            this._logoutCount = logoutCount;
            this._balance = balance;
            this._lastLogin = lastLogin;
            this._loginCount = loginCount;
        }

        public string accountID
        {
            get 
            {
                return _accountID; 
            }
        }

        public int loginCount
        {
            get
            {
                return _loginCount;
            }
        }

        public int logoutCount
        {
            get
            {
                return _logoutCount;
            }
        }

        public DateTime lastLogin
        {
            get
            {
                return _lastLogin;
            }
        }

        public DateTime creationDate
        {
            get
            {
                return _creationDate;
            }
        }

        public decimal balance
        {
            get
            {
                return _balance;
            }
        }

        public decimal openBalance
        {
            get
            {
                return _openBalance;
            }
        }

        public string profileID
        {
            get
            {
                return _userId;
            }
        }
    }
}