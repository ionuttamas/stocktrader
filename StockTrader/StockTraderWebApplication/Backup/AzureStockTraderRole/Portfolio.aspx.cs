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
//                      The StockTraderWebApplicationConfigurationImplementation and StockTraderWebApplicationSettings projects
//                      are projects in this solution that are part of the Configuration Service implementation for StockTrader 6.  A version of StockTrader that did not implement
//                      the Configuration Service would not have these projects, and code would be written instead to load <appSettings> from web.config; as well as its own WCF logic
//                      for remote calls, vs. using the base classes (through inheritence) that the Configuration Service provides.
//                      The StockTraderWebApplicationServiceClient is the WCF client for the remote BSL layer, and inherits from the Configuration Service 6
//                      base class, a client that provides additional performance, load balancing and failover functionality for WCF services.  
//              


using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;

namespace Trade.Web
{
    /// <summary>
    /// Displays Portfolio info for a user. 
    /// </summary>
    public partial class Portfolio : System.Web.UI.Page
    {
        public TotalHoldingsUI totalHoldings;

        protected void Page_Load(object sender, EventArgs e)
        {
            string userid = HttpContext.Current.User.Identity.Name;
            if (userid == null)
                logout();
            BSLClient businessServicesClient = new BSLClient();
            totalHoldings = businessServicesClient.getHoldings(userid);
            List<HoldingDataUI> holdingList = totalHoldings.holdings;
            numOfHoldings.Text = totalHoldings.holdings.Count.ToString();
            PortfolioRepeater.DataSource = holdingList;
            PortfolioRepeater.DataBind();
        }

        private void logout()
        {
            FormsAuthentication.SignOut();
            Response.Redirect(Settings.PAGE_LOGIN, true);
        }
        protected void QuoteButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(Settings.PAGE_QUOTES + "?symbols=" + symbols.Text, true);
        }
}
}

