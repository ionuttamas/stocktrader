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
using System.ServiceModel;
using Trade.BusinessServiceDataContract;
using Trade.DALFactory;
using Trade.IDAL;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorServiceConfigurationSettings;

namespace Trade.OrderProcessorImplementation
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class QuoteService : IQuoteService
    {
        /// <summary>
        /// Determines whether this instance is online. No-op.
        /// </summary>
        public void isOnline()
        {
        }

        /// <summary>
        /// Gets the quote for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public QuoteDataModel GetQuote(string symbol)
        {
            IMarketSummary marketSummary = MarketSummary.Create(Settings.DAL);
            
            try
            {
                marketSummary.Open(Settings.TRADEDB_SQL_CONN_STRING);
                return marketSummary.getQuote(symbol);
            }
            finally
            {
                marketSummary.Close();
            }
        }

        /// <summary>
        /// Gets quotes for the given symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        public List<QuoteDataModel> GetQuotes(string symbols)
        {
            IMarketSummary marketSummary = MarketSummary.Create(Settings.DAL);
            
            try
            {
                marketSummary.Open(Settings.TRADEDB_SQL_CONN_STRING);
                return marketSummary.getQuotes(symbols);
            }
            finally
            {
                marketSummary.Close();
            }
        }

        /// <summary>
        /// Gets the market summary data.
        /// </summary>
        public MarketSummaryDataModelWS GetMarketSummaryData()
        {
            IMarketSummary marketSummary = MarketSummary.Create(Settings.DAL);
            
            try
            {
                marketSummary.Open(Settings.TRADEDB_SQL_CONN_STRING);
                return marketSummary.getMarketSummaryData();
            }
            finally
            {
                marketSummary.Close();
            }
        }
    }
}
