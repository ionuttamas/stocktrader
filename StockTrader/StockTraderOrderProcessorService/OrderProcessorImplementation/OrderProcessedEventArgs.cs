using System;
using Trade.BusinessServiceDataContract;

namespace Trade.OrderProcessorImplementation
{
    /// <summary>
    /// Used to reply to the BSL when OPS is running in-proc with BSL.
    /// </summary>
    public class OrderProcessedEventArgs : EventArgs
    {
        public OrderProcessedEventArgs(OrderResponseDataModel orderResponseDataModel)
        {
            Response = orderResponseDataModel;
        }

        public OrderResponseDataModel Response { get; private set; }
    }
}
