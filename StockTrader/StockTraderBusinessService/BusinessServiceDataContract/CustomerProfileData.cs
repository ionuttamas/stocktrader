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
// The AccountProfileDataModel class, part of the DataContract for the StockTrader Business Services Layer.
//======================================================================================================


using System.Runtime.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This class is part of the WCF Data Contract for StockTrader Business Services.
    /// It defines the class used as the data model for account profile information. 
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://trade.samples.websphere.ibm.com", TypeName = "AccountProfileDataBean")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://trade.samples.websphere.ibm.com", Name = "AccountProfileDataBean")]
    public sealed class AccountProfileDataModel
    {
        private string _userId;
        private string _password;
        private string _fullName;
        private string _address;
        private string _email;
        private string _creditCard;
            
        public AccountProfileDataModel()
        {
        }

    public AccountProfileDataModel(
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "userID", Order = 1, IsNullable = false)]
        [DataMember(IsRequired=false,Name="userID",Order=1)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "password", Order = 2, IsNullable = false)]
        [DataMember(IsRequired=false,Name="password",Order=2)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "fullName", Order = 3, IsNullable = false)]
        [DataMember(IsRequired=false,Name="fullName",Order=3)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "address", Order = 4, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "address", Order = 4)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "email", Order = 5, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "email", Order = 5)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "creditCard", Order = 6, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "creditCard", Order = 6)]
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