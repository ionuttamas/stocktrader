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
// An interface implemented by the DAL.
//======================================================================================================


using Trade.BusinessServiceDataContract;

namespace Trade.IDAL
{
    public interface IOrder
    {
        void BeginADOTransaction();
        void RollBackTransaction();
        void CommitADOTransaction();
        void Open(string connString);
        void Close();
        OrderDataModel createOrder(string userID, string symbol, string orderType, double quantity, string holdingID);
        string createHolding(OrderDataModel order);
        OrderDataModel getOrder(string userID, string orderID);
        void updateHolding(string userID, string holdingid, double quantity);
        void updateOrder(OrderDataModel order);
        void deleteHolding(string userID, string holdingid);
        QuoteDataModel getQuoteForUpdate(string symbol);
        void closeOrder(OrderDataModel order);
        void updateAccountBalance(string accountID, decimal total);
        void updateStockPriceVolume(double quantity, QuoteDataModel quote);
        HoldingDataModel getHoldingForUpdate(string userID, string orderID);
        HoldingDataModel getHolding(string userID, string holdingID);
        void getSQLContextInfo();
    }
}