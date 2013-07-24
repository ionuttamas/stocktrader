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


using System.Collections.Generic;
using Trade.BusinessServiceDataContract;

namespace Trade.IDAL
{
    public interface ICustomer
    {
        void BeginADOTransaction();
        void RollBackTransaction();
        void CommitADOTransaction();
        void Open(string connString);
        void Close();
        void getSQLContextInfo();
        void updateAccountBalance(string userID, decimal total);
        AccountDataModel login(string userId, string password, bool useSaltedHash);
        void logOutUser(string userID);
        AccountDataModel getCustomerByUserID(string UserID);
        AccountProfileDataModel getAccountProfileData(string userID);
        List<OrderDataModel> getClosedOrders(string userId);
        List<HoldingDataModel> getHoldings(string userID);
        HoldingDataModel getHoldingForUpdate(string userID, string orderID);
        HoldingDataModel getHolding(string userID, string holdingID);
        void insertAccount(AccountDataModel customer);
        void insertAccountProfile(AccountProfileDataModel customerprofile, bool useSaltedHash);
        List<OrderDataModel> getOrders(string userID, bool top, int maxTop, int maxDefault);
        AccountProfileDataModel update(AccountProfileDataModel customerprofile, bool useSaltedHash);

        int? getAccountDeviceTopicIndex(string userID, string deviceID);
        int getNewAccountDeviceTopicIndex();
        void insertAccountDeviceTopicIndex(string userID, string deviceID, int topicIndex);
        bool validate(string userid, string password, bool useSaltedHash);
    }
}
