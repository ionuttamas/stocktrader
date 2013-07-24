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
// The Order Processor Service implementation class/business logic.
//======================================================================================================

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Trade.BusinessServiceDataContract;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorServiceConfigurationSettings;

namespace Trade.OrderProcessorImplementation
{
    /// <summary>
    /// Service class which implements the Order Processor service contract.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class OrderProcessor : IOrderProcessor
    {
        public OrderProcessor()
        {
            Interlocked.Increment(ref Settings.invokeCount);
        }

        /// <summary>
        /// Interesting in that this service contract is a one-way contract. However, it
        //  turns out this online check works fine in that we are loosely couple when working with MSMQ
        //  and still tightly coupled on one-way TCP or HTTP bindings.  The behavior is such
        //  that this method will fail from calling client only on TCP/HTTP one-way (Async)
        //  bindings, which happens regardless on any method call; but will not fail on MSMQ bindings 
        //  if the messaging service is running.  If MSMQ messaging service is not running at the endpoint, 
        //  we will have endpoint not found detection as desired.
        /// </summary>
        public void isOnline()
        {
            //We will not count this online check from Remote client as an activation, since its not a real operation.  Ultimately,
            //there is a TODO here, which is apply a WCF endpoint behavior, such that we count actual requests at the service op level.
            //Also note while this will be called from ConfigWeb for admin procedures (and discounted here); it will not be called
            //when using an external load balancer; or if the known node count=1.  There is no point in this check, if there is no
            //node with a different address to failover to.  So, for on-premise, this kicks in with 2 or more nodes, with the
            //benefit of service-op level failover.  With external load balancers, its up to them to decide at what level
            //they check online status.  It might be at the (virtual) machine level; or at the endpoint level.
            Interlocked.Decrement(ref Settings.invokeCount);
        }

        /// <summary>
        /// SubmitOrder service operation is a service operation that processes order messages from the client
        /// coming in via TCP, HTTP, or a Service Bus WCF binding from either the BSL or another remote instance
        /// </summary>
        /// <param name="order">Order to submit.</param>
        public void SubmitOrder(OrderDataModel order)
        {
            new OrderProcessorEngine().SubmitOrder(order);
            CompleteReceiveContext();
        }

        /// <summary>
        /// Completes the receive context (if enabled).
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
