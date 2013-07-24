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
using System.ServiceModel.Channels;
using ConfigService.LoadBalancingClient;
using Trade.BusinessServiceDataContract;
using Trade.OrderProcessorContract;

namespace Trade.OrderProcessorAsyncClient
{
    public class QuoteServiceClient : IQuoteService
    {
        private readonly Client _client;

        /// <summary>
        /// This will initialize the correct client/endpoint based on the QuoteMode setting the user has set
        /// in the repository.
        /// </summary>
        /// <param name="quoteMode">The quote mode, determines what type of binding/remote interface is used for communication.</param>
        /// <param name="settingsInstance">instance of this host's Settings class.</param>
        public QuoteServiceClient(string quoteMode, object settingsInstance)
        {
            _client = new Client(quoteMode, settingsInstance);
        }

        /// <summary>
        /// This returns the base channel type, cast to the specific contract type.
        /// </summary>
        public IQuoteService Channel
        {
            get
            {
                return (IQuoteService)_client.Channel;
            }
            set
            {
                _client.Channel = (IChannel)value;
            }
        }

        /// <summary>
        /// Determines whether this instance is online.
        /// </summary>
        public void isOnline()
        {
            try
            {
                this.Channel.isOnline();
            }
            catch (Exception)
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets a quote.
        /// </summary>
        /// <param name="symbol"></param>
        public QuoteDataModel GetQuote(string symbol)
        {
            try
            {
                return this.Channel.GetQuote(symbol);
            }
            catch
            {
                this.Channel = null;
                throw;
            }

        }

        /// <summary>
        /// Gets quotes.
        /// </summary>
        /// <param name="symbols"></param>
        public List<QuoteDataModel> GetQuotes(string symbols)
        {
            try
            {
                return this.Channel.GetQuotes(symbols);
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Gets market summary data.
        /// </summary>
        public MarketSummaryDataModelWS GetMarketSummaryData()
        {
            try
            {
                return this.Channel.GetMarketSummaryData();
            }
            catch (Exception)
            {
                this.Channel = null;
                throw;
            }
        }
    }
}
