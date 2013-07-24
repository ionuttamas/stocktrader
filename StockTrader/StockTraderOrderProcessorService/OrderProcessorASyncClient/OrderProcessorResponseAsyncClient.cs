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

using System.ServiceModel.Channels;
using ConfigService.LoadBalancingClient;
using Trade.BusinessServiceDataContract;
using Trade.OrderProcessorContract;

namespace Trade.OrderProcessorAsyncClient
{
    public class OrderProcessorResponseAsyncClient : IOrderProcessorResponse
    {
        private readonly Client _opsclient;

        /// <summary>
        /// This will initialize the correct client/endpoint based on the OrderResponseMode setting the user has set
        /// in the repository.
        /// </summary>
        /// <param name="orderMode">The order mode, determines what type of binding/remote interface is used for communication.</param>
        /// <param name="settingsInstance">instance of this host's Settings class.</param>
        public OrderProcessorResponseAsyncClient(string orderMode, object settingsInstance)
        {
            _opsclient = new Client(orderMode, settingsInstance);
        }

        /// <summary>
        /// This returns the base channel type, cast to the specific contract type.
        /// </summary>
        public IOrderProcessorResponse Channel
        {
            get
            {
                return (IOrderProcessorResponse)_opsclient.Channel;
            }
            set
            {
                _opsclient.Channel = (IChannel)value;
            }
        }

        public void OrderProcessed(OrderResponseDataModel orderResponse)
        {
            Channel.OrderProcessed(orderResponse);
        }

        public void isOnline()
        {
            Channel.isOnline();
        }
    }
}
