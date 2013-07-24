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

namespace Trade.OrderProcessorContract
{
    /// <summary>
    /// Exposes latest quote and market data via a request-response binding.
    /// </summary>
    [ServiceContract(Name = "QuoteService", Namespace = "http://Trade.TraderOrderHost")]
    public interface IQuoteService
    {
        /// <summary>
        /// Determines whether this instance is online.
        /// </summary>
        [OperationContract(Action = "isOnline", IsOneWay = true)]
        void isOnline();

        /// <summary>
        /// Gets the quote for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        [OperationContract]
        QuoteDataModel GetQuote(string symbol);

        /// <summary>
        /// Gets quotes for the given symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        [OperationContract]
        List<QuoteDataModel> GetQuotes(string symbols);

        /// <summary>
        /// Gets the market summary data.
        /// </summary>
        [OperationContract]
        MarketSummaryDataModelWS GetMarketSummaryData();
    }
}
