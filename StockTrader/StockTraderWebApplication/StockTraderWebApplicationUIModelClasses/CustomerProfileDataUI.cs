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
    /// Model class for displaying account profile data in a web page.
    /// </summary>
    [Serializable]
    public sealed class AccountProfileDataUI
    {
        private string _userId;
        private string _password;
        private string _fullName;
        private string _address;
        private string _email;
        private string _creditCard;
            
        public AccountProfileDataUI()
        {
        }

        public AccountProfileDataUI(
		    string userid,
            string password,
            string fullname,
            string address,  
            string email,        
            string creditcard
            )
	    {
            this._userId = userid;
            this._password = password;
            this._fullName = fullname;
            this._address = address;
            this._email = email;
            this._creditCard = creditcard;
	    }

        public string userID
        {
            get
            {
                return _userId;
            }
            set
            {
                this._userId = value;
            }
        }

        public string password
        {
            get
            {
                return _password;
            }
            set
            {
                this._password = value;
            }
        }

        public string fullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                this._fullName = value;
            }
        }

        public string address
        {
            get
            {
                return _address;
            }
            set
            {
                this._address = value;
            }
        }

        public string email
        {
            get
            {
                return _email;
            }
            set
            {
                this._email = value;
            }
        }

        public string creditCard
        {
            get
            {
                return _creditCard;
            }
            set
            {
                this._creditCard = value;
            }
        }
    }
}