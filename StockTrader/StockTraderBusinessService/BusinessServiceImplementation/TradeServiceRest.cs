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
using System.IdentityModel.Claims;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Trade.BusinessServiceDataContract;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// REST implementation of the Trade Service. The REST version of the service can be accessed by non-trusted clients
    /// so additional authorization is required.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class TradeServiceBSLRest : TradeServiceBSL
    {
        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override List<OrderDataModel> getOrders(string userID)
        {
            Authorize(userID);
            return base.getOrders(userID);
        }

        /// <summary>
        /// Gets the account data.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override AccountDataModel getAccountData(string userID)
        {
            Authorize(userID);
            return base.getAccountData(userID);
        }

        /// <summary>
        /// Gets the account profile data.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override AccountProfileDataModel getAccountProfileData(string userID)
        {
            Authorize(userID);
            return base.getAccountProfileData(userID);
        }

        /// <summary>
        /// Updates the account profile.
        /// </summary>
        /// <param name="profileData">The profile data.</param>
        public override AccountProfileDataModel updateAccountProfile(AccountProfileDataModel profileData)
        {
            Authorize(profileData.userID);
            return base.updateAccountProfile(profileData);
        }

        /// <summary>
        /// Logouts the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override void logout(string userID)
        {
            Authorize(userID);
            base.logout(userID);
        }

        /// <summary>
        /// Buys the specified symbol.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="orderProcessingMode">The order processing mode.</param>
        public override OrderDataModel buy(string userID, string symbol, double quantity, int orderProcessingMode)
        {
            Authorize(userID);
            return base.buy(userID, symbol, quantity, orderProcessingMode);
        }

        /// <summary>
        /// Sells the specified holding.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        /// <param name="orderProcessingMode">The order processing mode.</param>
        public override OrderDataModel sell(string userID, string holdingID, int orderProcessingMode)
        {
            Authorize(userID);
            return base.sell(userID, holdingID, orderProcessingMode);
        }

        /// <summary>
        /// Gets the holdings for the user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override List<HoldingDataModel> getHoldings(string userID)
        {
            Authorize(userID);
            return base.getHoldings(userID);
        }

        /// <summary>
        /// Registers the device for the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="deviceID">The device ID.</param>
        public override DeviceDataModel registerDevice(string userID, string deviceID)
        {
            Authorize(userID);
            return base.registerDevice(userID, deviceID);
        }

        /// <summary>
        /// Gets the closed orders for the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override List<OrderDataModel> getClosedOrders(string userID)
        {
            Authorize(userID);
            return base.getClosedOrders(userID);
        }

        /// <summary>
        /// Gets the holding specified by ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        public override HoldingDataModel getHolding(string userID, string holdingID)
        {
            Authorize(userID);
            return base.getHolding(userID, holdingID);
        }

        /// <summary>
        /// Gets the top *n* orders for a user.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public override List<OrderDataModel> getTopOrders(string userID)
        {
            Authorize(userID);
            return base.getTopOrders(userID);
        }

        /// <summary>
        /// Sells part or all of the specified holding.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="holdingID">The holding ID.</param>
        /// <param name="quantity">The quantity.</param>
        public override OrderDataModel sellEnhanced(string userID, string holdingID, double quantity)
        {
            Authorize(userID);
            return base.sellEnhanced(userID, holdingID, quantity);
        }

        /// <summary>
        /// Authorizes the specified user ID by comparing it to the current principal. If the authentication fails,
        /// through a REST-style exception (i.e. empty response, correct HTTP status code, correct HTTP header).
        /// </summary>
        /// <param name="userID">The user ID.</param>
        private static void Authorize(string userID)
        {
            var claim = ServiceSecurityContext.Current.AuthorizationContext.ClaimSets.SelectMany(x => x.FindClaims("urn:Claim:UserAccess", Rights.PossessProperty)).SingleOrDefault();
            if (claim == null || ((string)claim.Resource != "*" && (string)claim.Resource != userID))
            {
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext.Current.OutgoingResponse.Headers[HttpResponseHeader.WwwAuthenticate] = "Basic realm=\"site\"";
                }
                throw new WebFaultException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
