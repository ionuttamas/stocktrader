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
// The OrderProcessor ServiceContract, which is quite simple.
//======================================================================================================

using System.ServiceModel;
using Trade.BusinessServiceDataContract;

namespace Trade.OrderProcessorContract
{
    /// <summary>
    /// This is the service contract for the Order Processor Service. It defines the service layer operations
    /// that are separately implemented in the implementation class.
    /// </summary> 
    [ServiceContract(Name = "OrderProcessorService", Namespace = "http://Trade.TraderOrderHost")]
    public interface IOrderProcessor
    {
        //IsOneWay Marks Methods as Asynchrounous.
        //This is our non-transacted method for async calls via http, tcp, service bus.
        [OperationContract(Action = "SubmitOrder", IsOneWay = true)]
        void SubmitOrder(OrderDataModel order);

        /// <summary>
        /// Online check method.
        /// </summary>
        [OperationContract(Action = "isOnline", IsOneWay = true)]
        void isOnline();
    }
}

