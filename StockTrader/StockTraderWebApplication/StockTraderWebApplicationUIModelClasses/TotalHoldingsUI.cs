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

using System.Collections.Generic;
using Trade.StockTraderWebApplicationSettings;

namespace Trade.StockTraderWebApplicationModelClasses
{
    /// <summary>
    /// Model class for displaying special/augmented holding data in a web page.
    /// </summary>
    public sealed class TotalHoldingsUI
    {
        private List<HoldingDataUI> _holdings;
        private decimal _gain;
        private decimal _marketValue;
        private decimal _basis;
        private int _uniqueStockCount;
        private int _totalHoldingsCount;

        public TotalHoldingsUI()
        {
        }

        public TotalHoldingsUI(List<HoldingDataUI>holdings, decimal marketValue, decimal basis, decimal gain)
        {
            this._holdings = holdings;
            this._gain = gain;
            this._marketValue = marketValue;
            this._basis = basis;
        }

        public TotalHoldingsUI(List<HoldingDataUI> holdings, decimal marketValue, decimal basis, decimal gain, int uniqueStockCount, int totalHoldingsCount)
        {
            this._holdings = holdings;
            this._gain = gain;
            this._marketValue = marketValue;
            this._basis = basis;
            this._uniqueStockCount = uniqueStockCount;
            this._totalHoldingsCount = totalHoldingsCount;
        }

        public List<HoldingDataUI> holdings
        {
            get
            {
                return _holdings;
            }
        }

        public decimal marketValue
        {
            get
            {
                return _marketValue;
            }
        }

        public decimal gain
        {
            get
            {
                return _gain;
            }
        }

        public decimal basis
        {
            get
            {
                return _basis;
            }

            set
            {
                this._basis = value;
            }
        }

        public int uniqueStockCount
        {
            get
            {
                return _uniqueStockCount;
            }

            set
            {
                this._uniqueStockCount = value;
            }
        }

        public int totalHoldingsCount
        {
            get
            {
                return _totalHoldingsCount;
            }

            set
            {
                this._totalHoldingsCount = value;
            }
        }
        
        public string gainWithArrow
        {
            get
            {
                if (this._gain > 0)
                    return string.Format("<div style=\"color:#palegreen\">{0:C}" + Settings.UPARROWLINK + "</div>", this._gain);
                else 
                    if (this._gain <0) 
                        return string.Format("<div style=\"color:#A40707\">{0:C}" + Settings.DOWNARROWLINK + "</div>", this._gain);
                    else
                        return string.Format("{0:C}", this._gain);
            }
        }
    }
}