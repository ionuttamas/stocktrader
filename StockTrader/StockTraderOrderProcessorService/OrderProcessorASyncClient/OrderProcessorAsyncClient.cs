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
// This is the client for the OrderProcessorService.
//======================================================================================================


using System;
using System.Diagnostics;
using System.ServiceModel.Channels;
using ConfigService.LoadBalancingClient;
using ConfigService.ServiceConfigurationUtility;
using Trade.BusinessServiceDataContract;
using Trade.OrderProcessorContract;

namespace Trade.OrderProcessorAsyncClient
{
    /// <summary>
    /// This is the WCF client class for the remote Order Processor Services. This class implements channel initialization and
    /// load balancing/failover logic across cached channel instances specific to the Configuration Management/Node services
    /// implemented in StockTrader via the LoadBalancingClient.Client class, now re-used across all clients 
    /// implementing the configuration service. 
    /// </summary>
    public class TradeOrderServiceAsyncClient : IOrderProcessor
    {
        private readonly object _settingsInstance;
        private readonly Client _client;

        /// <summary>
        /// This will initialize the correct client/endpoint based on the OrderMode setting the user has set
        /// in the repository.
        /// </summary>
        /// <param name="orderMode">The order mode, determines what type of binding/remote interface is used for communication.</param>
        /// <param name="settingsInstance">instance of this host's Settings class.</param>
        public TradeOrderServiceAsyncClient(string orderMode, object settingsInstance)
        {
            _settingsInstance = settingsInstance;
            _client = new Client(orderMode, settingsInstance);
        }

        /// <summary>
        /// This returns the base channel type, cast to the specific contract type.
        /// </summary>
        public IOrderProcessor Channel
        {
            get
            {
                return (IOrderProcessor)_client.Channel;
            }
            set
            {
                _client.Channel = (IChannel)value;
            }
        }

        /// <summary>
        /// The online check method.
        /// </summary>
        public void isOnline()
        {
            try
            {
                this.Channel.isOnline();
                return;
            }
            catch
            {
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// Submits an order to a non-transacted queue.
        /// </summary>
        /// <param name="order"></param>
        public void SubmitOrder(OrderDataModel order)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    this.Channel.SubmitOrder(order);
                    return;
                }
                catch
                {
                    count++;
                    this.Channel = null;
                    if (count == 4)
                        throw;
                }
            }
        }

        /// <summary>
        /// This method sends an order off to the order processor service, in an async fashion depending on OrderMode setting.
        /// </summary>
        /// <param name="order">Order to submit.</param>
        public void processOrderASync(OrderDataModel order)
        {
            try
            {
                SubmitOrder(order);
            }
            catch (Exception e)
            {
                string innerException = null;
                if (e.InnerException != null)
                    innerException = e.InnerException.ToString();
                ConfigUtility.writeErrorConsoleMessage(e.ToString() + "\nInner Exception: " + innerException, EventLogEntryType.Error, true, _settingsInstance);
                this.Channel = null;
                throw;
            }
        }

        /// <summary>
        /// The OPS service will use this when running in OrderProcessingBehavior="Forward".  It fowards the order to a another
        /// remote instance of the Order Processing Service, perhaps over a different binding even, and does not actually
        /// process the order.  This makes this instance an intermediary.
        /// </summary>
        /// <param name="order">Order to submit.</param>
        public void forwardOrder(OrderDataModel order)
        {
            try
            {
                SubmitOrder(order);
                return;
            }
            catch (Exception e)
            {
                string innerException = null;
                if (e.InnerException != null)
                    innerException = e.InnerException.InnerException.ToString();
                ConfigUtility.writeErrorConsoleMessage(e.ToString() + "\nInner Exception: " + innerException, EventLogEntryType.Error, true, _settingsInstance);
                this.Channel = null;
                throw;
            }
        }
    }
}