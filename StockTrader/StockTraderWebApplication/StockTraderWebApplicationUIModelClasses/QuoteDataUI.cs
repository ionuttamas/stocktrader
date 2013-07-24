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

using Trade.StockTraderWebApplicationSettings;

namespace Trade.StockTraderWebApplicationModelClasses
{
    /// <summary>
    /// Model class for displaying quote data in a web page.
    /// </summary>
    public sealed class QuoteDataUI
    {
        private string _symbol;
        private string _companyName;
        private double _volume;
        private decimal _price;
        private decimal _open;
        private decimal _low;
        private decimal _high;
        private double _change;
        
        public QuoteDataUI()
        {
        }

        public QuoteDataUI(string symbol, string companyName, double volume, decimal price, decimal open, decimal low, decimal high, double change)
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

        public string quoteLink
        {
            get
            {
                return "<a href=\""+ Settings.PAGE_QUOTES + "?symbols=" + _symbol + "\">" + _symbol + "</a>";
            }
        }

        public string gainWithArrow
        {
            get
            {
                if (this._change > 0)
                    return string.Format("<div id=\"Gain\" style=\"color:palegreen" + "\">{0:C}" + Settings.UPARROWLINK + "</div>", this._change);
                else if (this._change < 0)
                    return string.Format("<div id=\"Gain\" style=\"color:#A40707" + "\">{0:C}" + Settings.DOWNARROWLINK + "</div>", this._change);
                else
                    return
                        string.Format("{0:C}", this._change);
            }
        }

        public string priceWithArrow
        {
            get
            {
                if (this._change > 0)
                    return string.Format("<span id=\"Gain\" style=\"color:palegreen" + "\">{0:C}" + Settings.UPARROWLINK + "</span>", this._price);
                else
                    if (this._change < 0)
                        return string.Format("<span id=\"Gain\" style=\"color:#A40707" + "\">{0:C}" + Settings.DOWNARROWLINK + "</span>", this._price);
                    else
                        return string.Format("{0:C}", this._price);                         
            }
        }
    }
}