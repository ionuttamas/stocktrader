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
// The MarketSummaryDataModelWS class, part of the DataContract for the StockTrader Business Services Layer.
//======================================================================================================


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This class is part of the WCF Data Contract for StockTrader Business Services.
    /// It defines the class used as the data model for market summary information. 
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://trade.samples.websphere.ibm.com", TypeName="MarketSummaryDataBeanWS")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://trade.samples.websphere.ibm.com",Name="MarketSummaryDataBeanWS")]
   	public sealed class MarketSummaryDataModelWS
	{
        private decimal _tsia;
        private decimal _openTSIA;
        private double _volume;
        private List<QuoteDataModel> _topGainers;
        private List<QuoteDataModel> _topLosers;
        private DateTime _summaryDate;
       
        public MarketSummaryDataModelWS()
        {
        }

        public MarketSummaryDataModelWS(decimal tSIA, decimal openTSIA, double volume, List<QuoteDataModel> topGainers, List<QuoteDataModel> topLosers)
        {
            this._tsia = tSIA;
            this._openTSIA = openTSIA;
            this._volume = volume;
            this._topGainers = topGainers;
            this._topLosers = topLosers;
            this._summaryDate = DateTime.Now;
        }


        [System.Xml.Serialization.XmlElementAttribute(ElementName = "TSIA", Order = 1, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "TSIA", Order = 1)]
        public decimal TSIA
        {
            get
            {
                return _tsia;
            }

            set
            {
                this._tsia = value;
            }

        }

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "openTSIA", Order = 2, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "openTSIA", Order = 2)]
		public decimal openTSIA
		{
			get
			{
				return _openTSIA;
			}
			
			set
			{
				this._openTSIA = value;
			}
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "volume", Order = 3, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "volume", Order = 3)]
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

        [System.Xml.Serialization.XmlArray(ElementName = "topGainers", Order = 4, IsNullable = false), System.Xml.Serialization.XmlArrayItem(typeof(QuoteDataModel))]
        [DataMember(EmitDefaultValue = false,IsRequired = false, Name = "topGainers", Order = 4)]
		public List<QuoteDataModel> topGainers
		{
			get
			{
				return _topGainers;
			}
			
			set
			{
				this._topGainers = value;
			}
			
		}

        [System.Xml.Serialization.XmlArray(ElementName = "topLosers", Order = 5, IsNullable = false), System.Xml.Serialization.XmlArrayItem(typeof(QuoteDataModel))]
        [DataMember(EmitDefaultValue = false,IsRequired = false, Name = "topLosers", Order = 5)]
		public List<QuoteDataModel> topLosers
		{
			get
			{
				return _topLosers;
			}
			
			set
			{
				this._topLosers = value;
			}
			
		}

        [System.Xml.Serialization.XmlElementAttribute(ElementName = "summaryDate", Order = 6, IsNullable = false)]
        [DataMember(IsRequired = false, Name = "summaryDate", Order = 6)]
		public DateTime summaryDate
		{
			get
			{
				return _summaryDate;
			}
			
			set
			{
				this._summaryDate = value;
			}
		}
    }
}