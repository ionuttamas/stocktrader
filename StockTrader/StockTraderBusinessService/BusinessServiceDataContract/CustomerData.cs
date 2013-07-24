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
// The AccountDataModel class, part of the DataContract for the StockTrader Business Services Layer.
//======================================================================================================

using System;
using System.Runtime.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This class is part of the WCF Data Contract for StockTrader Business Services.
    /// It defines the class used as the data model for account information. 
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://trade.samples.websphere.ibm.com", TypeName="AccountDataBean")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://trade.samples.websphere.ibm.com",Name="AccountDataBean")]
    public sealed class AccountDataModel
    {
        private string _accountID; 		    
        private DateTime _creationDate; 	
        private string _userId;
        private decimal _openBalance;
        private int _logoutCount;
        private decimal _balance;
        private DateTime _lastLogin;
        private int _loginCount;
            
        public AccountDataModel()
        {
        }

        public AccountDataModel(string accountID, string userid,
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "accountID", Order = 1, IsNullable=false)]
        [DataMember(IsRequired = false, Name = "accountID", Order = 1)]
        public string accountID
        {
            // Properties
            get { return _accountID; }
            set
            {
                this._accountID = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "loginCount", Order = 2, IsNullable=false)]
        [DataMember(IsRequired = false, Name = "loginCount", Order = 2)]
        public int loginCount
        {
            get
            {
                return _loginCount;
            }
            set
            {
                this._loginCount = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(ElementName = "logoutCount", Order = 3, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "logoutCount", Order = 3)]
        public int logoutCount
        {
            get
            {
                return _logoutCount;
            }
            set
            {
                this._logoutCount = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "lastLogin", Order = 4, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "lastLogin", Order = 4)]
        public DateTime lastLogin
        {
            get
            {
                return _lastLogin;
            }
            set
            {
                this._lastLogin = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "creationDate", Order = 5, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "creationDate", Order = 5)]
        public DateTime creationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                this._creationDate= value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "balance", Order = 6, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "balance", Order = 6)]
        public decimal balance
        {
            get
            {
                return _balance;
            }
            set
            {
                this._balance = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "openBalance", Order = 7, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "openBalance", Order = 7)]
        public decimal openBalance
        {
            get
            {
                return _openBalance;
            }
            set
            {
                this._openBalance = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "profileID", Order = 8, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "profileID", Order = 8)]
        public string profileID
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
    }
}