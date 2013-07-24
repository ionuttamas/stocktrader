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
using Trade.StockTraderWebApplicationSettings;

namespace Trade.StockTraderWebApplicationModelClasses
{
    /// <summary>
    /// Model class for displaying order data in a web page.
    /// </summary>
    public sealed class OrderDataUI
	{
        private string _orderID;
        private string _orderType;
        private string _orderStatus;
        private DateTime _openDate;
        private string _completionDate;
        private double _quantity;
        private decimal _price;
        private decimal _orderFee;
        private string _symbol;

        public OrderDataUI()
        {
        }

        public OrderDataUI(string orderID, string orderType, string orderStatus, DateTime openDate, string completionDate, double quantity, decimal price, decimal orderFee, string symbol)
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
        }

     	public string orderID
		{
			get
			{
				return _orderID;
			}
		}
        
        public string orderType
		{
			get
			{
				return _orderType;
			}
		}

 		public string orderStatus
		{
			get
			{
				return _orderStatus;
			}
		}

 		public DateTime openDate
		{
			get
			{
				return _openDate;
			}
			
		}

        public string completionDate
		{
			get
			{
				return _completionDate;
			}
		}

 		public double quantity
		{
			get
			{
				return _quantity;
			}
		}

 		public decimal price
		{
			get
			{
				return _price;
			}
		}

 		public decimal orderFee
		{
			get
			{
				return _orderFee;
			}
		}

        public decimal total
        {
            get
            {
                return Convert.ToDecimal(this._quantity)*this._price + this._orderFee;
            }
        }

 		public string symbol
		{
			get
			{
				return _symbol;
			}
		}

        public string quoteLink
        {
            get
            {
                return "<a href=\"" + Settings.PAGE_QUOTES + "?symbols=" + _symbol + "\">" + _symbol + "</a>";
            }
        }
	}
}