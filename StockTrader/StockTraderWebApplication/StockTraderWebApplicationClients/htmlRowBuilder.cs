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
using Trade.StockTraderWebApplicationModelClasses;

namespace Trade.StockTraderWebApplicationServiceClient
{
    /// <summary>
    /// Special class to construct a sorted list of holdings by stock symbol, pre-subtotaled and pre-formatted for ASPX repeater display.
    /// </summary>
    public class htmlRowBuilder
    {
        /// <summary>
        /// Logic to construct a sorted list of holdings by stock symbol, pre-subtotaled and pre-formatted for ASPX repeater display.
        /// </summary>
        /// <param name="holdingsUI">Holding data to format</param>
        public int buildPortfolioBySymbol(List<HoldingDataUI> holdingsUI)
        {
            if (holdingsUI == null || holdingsUI.Count < 1)
                return 0;
            string quoteSymbol = holdingsUI[0].quoteID;
            decimal quotePrice = holdingsUI[0].quotePriceDecimal;
            double subtotalquantity = 0;
            decimal subtotalmktvalue = 0;
            decimal subtotalbasis = 0;
            decimal subtotalgain = 0;
            int uniqueStockCount = 0;
            int count = holdingsUI.Count;
            int subtotaledlistcount = 0;
            for (int i = 0; i <= count; i++)
            {
                if (i == count)
                {
                    subtotaledlistcount--;
                }
                if (!quoteSymbol.Equals(holdingsUI[subtotaledlistcount].quoteID))
                {
                    uniqueStockCount += 1;
                    HoldingDataUI subtotalline = new HoldingDataUI(subtotalquantity, subtotalgain, subtotalmktvalue, subtotalbasis, quoteSymbol, quotePrice);
                    quoteSymbol = holdingsUI[subtotaledlistcount].quoteID;
                    quotePrice = holdingsUI[subtotaledlistcount].quotePriceDecimal;
                    subtotalgain = holdingsUI[subtotaledlistcount].gainDecimal;
                    subtotalquantity = holdingsUI[subtotaledlistcount].quantityDouble;
                    subtotalmktvalue = holdingsUI[subtotaledlistcount].marketValueDecimal;
                    subtotalbasis = holdingsUI[subtotaledlistcount].basisDecimal;
                    if (i != count)
                        holdingsUI[subtotaledlistcount].convertNumericsForDisplay(true);
                    else
                        subtotaledlistcount++;
                    holdingsUI.Insert(subtotaledlistcount++, subtotalline);
                    subtotaledlistcount++;
                }
                else
                {
                    subtotalgain += holdingsUI[subtotaledlistcount].gainDecimal;
                    subtotalquantity += Convert.ToDouble(holdingsUI[subtotaledlistcount].quantityDouble);
                    subtotalmktvalue += Convert.ToDecimal(holdingsUI[subtotaledlistcount].marketValueDecimal);
                    subtotalbasis += Convert.ToDecimal(holdingsUI[subtotaledlistcount].basisDecimal);
                    holdingsUI[subtotaledlistcount].convertNumericsForDisplay(true);
                    subtotaledlistcount++;
                }
            }
            return uniqueStockCount;  
        }
    }
}
