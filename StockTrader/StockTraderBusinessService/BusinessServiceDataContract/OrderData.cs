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
// The OrderDataModel class, part of the DataContract for the StockTrader Business Services Layer.
//======================================================================================================


using System;
using System.Runtime.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This class is part of the WCF Data Contract for StockTrader Business Services.
    /// It defines the class used as the data model for order information. 
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://trade.samples.websphere.ibm.com", TypeName="OrderDataBean")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://trade.samples.websphere.ibm.com",Name="OrderDataBean")]
    public sealed class OrderDataModel
	{
        private string _orderID;
        private string _orderType;
        private string _orderStatus;
        private DateTime _openDate;
        private DateTime _completionDate;
        private double _quantity;
        private decimal _price;
        private decimal _orderFee;
        private string _accountid;
        private string _holdingID;
        private string _symbol;
        private string _userID;

        public OrderDataModel()
        {
        }

        public OrderDataModel(string orderID, string orderType, string orderStatus, DateTime openDate, DateTime completionDate, double quantity, decimal price, decimal orderFee, string symbol, string userID)
        {
            this._orderID = orderID;
            this._orderType = orderType;
            this._orderStatus = orderStatus;
            this._openDate = openDate;
            this._completionDate = completionDate;
            this._quantity = quantity;
            this._price = price;
            this._orderFee = orderFee;
            this._symbol = symbol;
            this._userID = userID;
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "orderID", Order = 1, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "orderID", Order = 1)]
		public string orderID
		{
			get
			{
				return _orderID;
			}
			
			set
			{
				this._orderID = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "orderType", Order = 2, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "orderType", Order = 2)]
		public string orderType
		{
			get
			{
				return _orderType;
			}
			
			set
			{
				this._orderType = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "orderStatus", Order = 3, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "orderStatus", Order = 3)]
		public string orderStatus
		{
			get
			{
				return _orderStatus;
			}
			
			set
			{
				this._orderStatus = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "openDate", Order = 4, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "openDate", Order = 4)]
		public DateTime openDate
		{
			get
			{
				return _openDate;
			}
			
			set
			{
				this._openDate = value;
			}
			
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "completionDate", Order = 5, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "completionDate", Order = 5)]
        public DateTime completionDate
		{
			get
			{
				return _completionDate;
			}
			
			set
			{
				this._completionDate = (DateTime) value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "quantity", Order = 6, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "quantity", Order = 6)]
		public double quantity
		{
			get
			{
				return _quantity;
			}
			
			set
			{
				this._quantity = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "price", Order = 7, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "price", Order = 7)]
		public decimal price
		{
			get
			{
				return _price;
			}
			
			set
			{
				this._price = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "orderFee", Order = 8, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "orderFee", Order = 8)]
		public decimal orderFee
		{
			get
			{
				return _orderFee;
			}
			
			set
			{
				this._orderFee = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "symbol", Order = 9, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "symbol", Order = 9)]
		public string symbol
		{
			get
			{
				return _symbol;
			}
			
			set
			{
				this._symbol = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "userID", Order = 10, IsNullable = true)]
        [DataMember(IsRequired = false, Name = "userID", Order = 9)]
        public string userID
        {
            get
            {
                return _userID;
            }

            set
            {
                this._userID = value;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string accountID
        {
            get
            {
                return _accountid;
            }

            set
            {
                this._accountid = value;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string holdingID
        {
            get
            {
                return _holdingID;
            }

            set
            {
                this._holdingID = value;
            }
        }
	}
}