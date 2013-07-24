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
using System.Collections.Generic;
using Trade.StockTraderWebApplicationSettings;

namespace Trade.StockTraderWebApplicationModelClasses
{
    /// <summary>
    /// Model class for displaying holding data in a web page.
    /// </summary>
    public sealed class HoldingDataUI 
	{
        private string _holdingID;
        private string _quantity;
        private string _purchasePrice;
        private string _purchaseDate;
        private string _quoteID;
        private string _quotePrice;
        private string _marketValue;
        private string _basis;
        private string _gainWithArrow;
        private string _sellLink;
        private double _quantityDouble;
        private decimal _purchasePriceDecimal;
        private decimal _marketValueDecimal;
        private decimal _basisDecimal;
        private decimal _gainDecimal;
        private decimal _quotePriceDecimal;
        
        public HoldingDataUI()
        {
        }

        //Constructor used to convert a single HoldingDataModel to a new HoldingDataUI object
        public HoldingDataUI(string holdingID, double quantity, decimal purchasePrice, DateTime purchaseDate, string quoteID, decimal quotePrice)
        {
            this._holdingID = holdingID;
            this._quantity = string.Format("{0:0,0}", quantity);
            this._purchasePrice = string.Format("{0:C}", purchasePrice);
            this._purchaseDate = purchaseDate.ToString();
            this._quoteID = "<a  style=\"text-align:center;\" href=\"" + Settings.PAGE_QUOTES + "?symbols=" + quoteID + "\">" + quoteID + "</a>"; 
            this._quotePrice = string.Format("{0:C}", quotePrice);
        }

        //Constructor used to create a non-subtotal line
        public HoldingDataUI(string holdingID, double quantity, decimal purchasePrice, string purchaseDate, string quoteID, decimal quotePrice)
        {
            this._quotePriceDecimal = quotePrice;
            this._purchasePriceDecimal = purchasePrice;
            this._marketValueDecimal = quotePrice * (decimal)quantity;
            this._basisDecimal = purchasePrice * (decimal)quantity;
            this._gainDecimal = this._marketValueDecimal - this._basisDecimal;
            this._quantityDouble = quantity;
            this._quoteID = quoteID;
            this._holdingID = holdingID;
            this._purchaseDate = purchaseDate;
        }

        //Constructor used to create a subtotal line which we use in PortFolioByStock.aspx
        public HoldingDataUI(double quantity, decimal gain, decimal marketvalue,decimal basis, string quoteSymbol, decimal quotePrice)
        {
            string tdOpenRight = "<th class=\"InnerDataSubtotal\" style=\"text-align:right\">";
            string tdOpenCenter = "<th class=\"InnerDataSubtotal\" style=\"text-align:center\">";
            string tdClose = "</th>";
            this._holdingID = tdOpenCenter + tdClose;
            this._purchaseDate = tdOpenCenter + tdClose;
            this._sellLink = tdOpenCenter + tdClose;
            this._purchasePrice = tdOpenCenter + tdClose;
            this._quantity = tdOpenRight + string.Format("{0:0,0}", quantity) + tdClose;
            this._quoteID = tdOpenCenter + "<a href=\"" + Settings.PAGE_QUOTES + "?symbols=" + quoteSymbol + "\">" + quoteSymbol + "</a>" + tdClose;
            if (gain > 0)
                _gainWithArrow = string.Format(tdOpenRight + "<span id=\"Gain\" style=\"color:palegreen;" + "\">{0:C}" + Settings.UPARROWLINK + "</span>" + tdClose, gain);
            else
                if (gain < 0)
                    _gainWithArrow = string.Format(tdOpenRight + "<span id=\"Loss\" style=\"color:#A40707;" + "\">{0:C}" + Settings.DOWNARROWLINK + "</span>" + tdClose, gain);
                else
                    _gainWithArrow = string.Format(tdOpenRight + "{0:C}" + tdClose, gain);
            this._quotePrice = tdOpenRight + string.Format("{0:C}", quotePrice) + tdClose;
            this._marketValue = tdOpenRight + string.Format("{0:C}", marketvalue) + tdClose;
            this._basis = tdOpenRight + string.Format("{0:C}", basis) + tdClose;
        }

        //This method will either create the string fields without adding a leading and <TD> trailing </TD>
        //or it will add the leading <TD> and trailing </TD> based on bool parameter td.
        public void convertNumericsForDisplay(bool td)
        {
            string tdOpenRight = "";
            string tdOpenCenter = "";
            string tdClose = "";
            string selllink = "?action=sell&holdingid=";
            if (td)
            {
                tdOpenRight = "<td class=\"InnerData\" style=\"text-align:right\">";
                tdOpenCenter = "<td class=\"InnerData\" style=\"text-align:center;\">";
                tdClose = "</td>";
                selllink = "?action=sell&holdingid=";
            }
            this._sellLink = tdOpenCenter + "<a href=\"" + Settings.PAGE_TRADE + selllink + this._holdingID + "\">Sell</a>" + tdClose;
            this._quotePrice = tdOpenRight + string.Format("{0:C}", this._quotePriceDecimal) + tdClose;
            this._holdingID = tdOpenCenter + this._holdingID.ToString() + tdClose;
            this._quantity = tdOpenRight + string.Format("{0:0,0}", this._quantityDouble) + tdClose;
            this._purchasePrice = tdOpenRight + string.Format("{0:C}", this._purchasePriceDecimal) + tdClose;
            this._purchaseDate = tdOpenCenter + this._purchaseDate + tdClose;
            this._quoteID = tdOpenCenter + "<a href=\"" + Settings.PAGE_QUOTES + "?symbols=" + this._quoteID + "\">" + this._quoteID + "</a>" + tdClose;
            if (_gainDecimal > 0)
                _gainWithArrow = string.Format(tdOpenRight + "<span id=\"Gain\" style=\"color:palegreen" + "\">{0:C}" + Settings.UPARROWLINK + "</span>" + tdClose, this._gainDecimal);
            else
                if (_gainDecimal < 0)
                    _gainWithArrow = string.Format(tdOpenRight + "<span id=\"Loss\" style=\"color:#A40707" + "\">{0:C}" + Settings.DOWNARROWLINK + "</span>" + tdClose, this._gainDecimal);
                else
                    _gainWithArrow = string.Format(tdOpenRight + "{0:C}" + tdClose, this._gainDecimal);
            this._marketValue = tdOpenRight + string.Format("{0:C}", this._marketValueDecimal) + tdClose;
            this._basis = tdOpenRight + string.Format("{0:C}", this._basisDecimal) + tdClose;
        }

        public class HoldingDataUIComparer : IComparer<HoldingDataUI>
        {
            public enum ComparisonType
            { quoteID = 1 }
            private ComparisonType _comparisonType;
            public ComparisonType ComparisonMethod
            {
                get { return _comparisonType; }
                set { _comparisonType = value; }
            }

            #region IComparer Members

            public int Compare(HoldingDataUI x, HoldingDataUI y)
            {
                return x.CompareTo(y, _comparisonType);
            }

            #endregion
        }

        public int CompareTo(object obj)
        {
            if (obj is HoldingDataUI)
            {
                HoldingDataUI holding2 = (HoldingDataUI)obj;

                return _quoteID.CompareTo(holding2._quoteID);
            }
            else
                throw new ArgumentException("Object is not HoldingDataUI element.");
        }

        public int CompareTo(HoldingDataUI holding2, HoldingDataUIComparer.ComparisonType comparisonMethod)
        {
            switch (comparisonMethod)
            {
                case HoldingDataUIComparer.ComparisonType.quoteID:
                    return _quoteID.CompareTo(holding2._quoteID);
                default:
                    return _quoteID.CompareTo(holding2._quoteID);
            }
        }

        public string quotePrice
        {
            get
            {
                return _quotePrice;
            }

            set
            {
                this._quotePrice = value;
            }
        }

        public decimal quotePriceDecimal
        {
            get
            {
                return _quotePriceDecimal;
            }

            set
            {
                this._quotePriceDecimal = value;
            }
        }

        public decimal gainDecimal
        {
            get
            {
                return _gainDecimal;
            }
        }

        public string marketValue
        {
            get
            {
                return _marketValue;
            }
        }

        public decimal marketValueDecimal
        {
            get
            {
                return _marketValueDecimal;
            }
        }

        public string basis
        {
            get
            {
                return _basis;
            }
        }

        public decimal basisDecimal
        {
            get
            {
                return _basisDecimal;
            }
        }


        public string holdingID
		{
			get
			{
				return _holdingID;
			}
		}

        public string quantity
        {
            get
            {
                return _quantity;
            }

        }

        public double quantityDouble
		{
			get
			{
				return _quantityDouble;
			}
		}

		public string purchasePrice
		{
			get
			{
				return _purchasePrice;
			}
		}

		public string purchaseDate
		{
			get
			{
				return _purchaseDate;
			}
		}

		public string quoteID
		{
			get
			{
				return _quoteID;
			}
		}
        
        public string gainWithArrow
        {
            get
            {
                return this._gainWithArrow;
            }
        }
       
        public string sellLink
        {
            get
            {
                return this._sellLink;
            }
        }
    }
}
