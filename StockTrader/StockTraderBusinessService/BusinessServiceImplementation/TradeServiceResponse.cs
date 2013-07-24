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
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Trade.BusinessServiceDataContract;
using Trade.OrderProcessorContract;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// Order response processing service. Receives responses from the OPS and updates the user's account.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    public class TradeServiceBSLResponse : IOrderProcessorResponse
    {
        /// <summary>
        /// Updates an order after it has been processed by the OPS.
        /// </summary>
        /// <param name="orderResponse">The order response.</param>
        public void OrderProcessed(OrderResponseDataModel orderResponse)
        {
            if (orderResponse == null)
            {
                throw new ArgumentNullException("orderResponse");
            }

            try
            {
                Trace.WriteLine("Received: " + orderResponse.orderID);

                new TradeEngine().OrderProcessed(orderResponse);

                // Remove message from the service bus if processing is successful.
                CompleteReceiveContext();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error receiving " + orderResponse.orderID + ": " + ex);
                throw;
            }
        }

        /// <summary>
        /// Online check method.
        /// </summary>
        public void isOnline()
        {
            // no-op
        }

        /// <summary>
        /// Completes the receive context.
        /// </summary>
        private static void CompleteReceiveContext()
        {
            ReceiveContext context = null;
            if (ReceiveContext.TryGet(OperationContext.Current.IncomingMessageProperties, out context))
            {
                context.Complete(TimeSpan.FromSeconds(10.0));
            }
        }
    }
}
