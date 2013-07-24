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
// The HoldingDataModel class, part of the DataContract for the StockTrader Business Services Layer.
//======================================================================================================

using System;
using System.Runtime.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This class is part of the WCF Data Contract for StockTrader Business Services.
    /// It defines the class used as the data model for holding information. 
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://trade.samples.websphere.ibm.com",TypeName="HoldingDataBean")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://trade.samples.websphere.ibm.com",Name="HoldingDataBean")]
    public sealed class HoldingDataModel
	{
        private string _accountID;
        private string _holdingID;
        private double _quantity;
        private decimal _purchasePrice;
        private DateTime _purchaseDate;
        private string _quoteID;
        private decimal _price;

       
        public HoldingDataModel()
        {
        }

        public HoldingDataModel(string holdingID, string accountID, double quantity, decimal purchasePrice, DateTime purchaseDate, string quoteID, decimal price)
        {
            this._price = price;
            this._holdingID = holdingID;
            this._accountID = accountID;
            this._quantity = quantity;
            this._purchasePrice = purchasePrice;
            this._purchaseDate = purchaseDate;
            this._quoteID = quoteID;
        }

        public HoldingDataModel(string holdingID, double quantity, decimal purchasePrice, DateTime purchaseDate, string quoteID, string accountID, decimal price)
        {
            this._price = price;
            this._holdingID = holdingID;
            this._accountID = accountID;
            this._quantity = quantity;
            this._purchasePrice = purchasePrice;
            this._purchaseDate = purchaseDate;
            this._quoteID = quoteID;
        }

        [System.Xml.Serialization.XmlIgnore]
        public string AccountID
        {
            get
            {
                return _accountID;
            }

            set
            {
                this._accountID = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "holdingID", Order = 1, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "holdingID", Order = 1)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "quantity", Order = 2, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "quantity", Order = 2)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "purchasePrice", Order = 3, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "purchasePrice", Order = 3)]
		public decimal purchasePrice
		{
			get
			{
				return _purchasePrice;
			}
			
			set
			{
				this._purchasePrice = value;
			}
			
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "purchaseDate", Order = 4, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "purchaseDate", Order = 4)]
		public DateTime purchaseDate
		{
			get
			{
				return _purchaseDate;
			}
			
			set
			{
				this._purchaseDate = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "quoteID", Order = 5, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "quoteID", Order = 5)]
		public string quoteID
		{
			get
			{
				return _quoteID;
			}
			
			set
			{
				this._quoteID = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "price", Order = 6, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "price", Order = 6)]
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
    }
}