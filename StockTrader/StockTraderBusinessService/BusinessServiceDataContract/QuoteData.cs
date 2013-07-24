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
// The QuoteDataModel class, part of the DataContract for the StockTrader Business Services Layer.
//======================================================================================================


using System.Runtime.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This class is part of the WCF Data Contract for StockTrader Business Services.
    /// It defines the class used as the data model for quote information. 
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://trade.samples.websphere.ibm.com", TypeName="QuoteDataBean")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://trade.samples.websphere.ibm.com",Name="QuoteDataBean")]
  	public sealed class QuoteDataModel
	{
        private string _symbol;
        private string _companyName;
        private double _volume;
        private decimal _price;
        private decimal _open;
        private decimal _low;
        private decimal _high;
        private double _change;

        public QuoteDataModel()
        {
        }

        public QuoteDataModel(string symbol, string companyName, double volume, decimal price, decimal open, decimal low, decimal high, double change)
        {
            this._symbol = symbol;
            this._companyName = companyName;
            this._volume = volume;
            this._price = price;
            this._open = open;
            this._low = low;
            this._high = high;
            this._change = change;
        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "symbol", Order = 1, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "symbol", Order = 1)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "companyName", Order = 2, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "companyName", Order = 2)]
		public string companyName
		{
			get
			{
				return _companyName;
			}
			
			set
			{
				this._companyName = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "price", Order = 3, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "price", Order = 3)]
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

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "open", Order = 4, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "open", Order = 4)]
		public decimal open
		{
			get
			{
				return _open;
			}
			
			set
			{
				this._open = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "low", Order = 5, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "low", Order = 5)]
		public decimal low
		{
			get
			{
				return _low;
			}
			
			set
			{
				this._low = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "high", Order = 6, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "high", Order = 6)]
		public decimal high
		{
			get
			{
				return _high;
			}
			
			set
			{
				this._high = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "change", Order = 7, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "change", Order = 7)]
		public double change
		{
			get
			{
				return _change;
			}
			
			set
			{
				this._change = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "volume", Order = 8, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "volume", Order = 8)]
        public double volume
		{
			get
			{
				return _volume;
			}
			
			set
			{
				this._volume = value;
			}
		}
	}
}