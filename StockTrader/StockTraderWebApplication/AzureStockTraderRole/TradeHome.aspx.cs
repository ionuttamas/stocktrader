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
using System.Web;
using System.Web.Security;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;

namespace Trade.Web
{
    /// <summary>
    /// The info-loaded Home Page.
    /// </summary>
    public partial class TradeHome : System.Web.UI.Page
    {
        string userid;
        TotalHoldingsUI totalHoldings;
        AccountDataUI customer;
        IAsyncResult result1;
        IAsyncResult result2;
        GetHoldingsAsync caller1;
        GetCustomerAsync caller2;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultFocus = symbols.ClientID;
            
            //Must get/decrypt FormsAuthentication ticket on this page only to get session created date for display.  
            //Would recommend not displaying session create date if really do not need to; depending on
            //configured Forms authentication protection level, decrypting ticket might be expensive!
            //We are needing to do here only to maintain same functionality as WebSphere Trade 6.1
            //for consistency and comparative benchmarking reasons.  If not for this, would simply need to grab userid 
            //(from which all logic/queries in this app are driven off of), from httpcontext.  You will note
            //in other authenticated pages where we do not need to display session created date on the aspx page for the user,
            //we just get userid from a property on the child ClosedOrders.Ascx control, which has gotten it
            //from HttpContext.Current.User.Identity.Name.  This control is embedded on every authenticated page per
            //Trade 6.1 design. This also avoids use of server-side Session state object entirely.  Its not that this is 
            //much faster than session state, the BIG advantage is that for deploying to web farms/clusters you never
            //have to worry about using ASP.NET state server or database configuration; it's "cluster-safe/webfarm-ready"
            //out of the box.
            HttpCookie authcookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authcookie == null)
                logout();
            FormsAuthenticationTicket ticket = (FormsAuthenticationTicket)FormsAuthentication.Decrypt(authcookie.Value);
            if (ticket == null)
                logout();
            userid = ticket.Name;
            SessionCreateDate.Text = ticket.IssueDate.ToString();
            // Create the delegate.
            caller1 = new GetHoldingsAsync(GetHoldings);
            // Initiate the asychronous call.
            result1 = caller1.BeginInvoke(null, null);
            // Repeat
            caller2 = new GetCustomerAsync(GetCustomer);
            // Initiate the asychronous call.
            result2 = caller2.BeginInvoke(null, null);
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            totalHoldings = caller1.EndInvoke(result1);
            customer = caller2.EndInvoke(result2);
            decimal gain = (decimal)0.00;
            decimal percentGain = (decimal)0.00;
            Name.Text = customer.profileID + "!";
            AccountID.Text = customer.accountID.ToString();
            CreationDate.Text = customer.creationDate.ToString();
            LoginCount.Text = customer.loginCount.ToString();
            OpenBalance.Text = string.Format("{0:C}", customer.openBalance);
            Balance.Text = string.Format("{0:C}", customer.balance);
            NumHoldings.Text = totalHoldings.holdings.Count.ToString();
            HoldingsTotal.Text = string.Format("{0:C}", totalHoldings.marketValue);
            decimal totalcashandholdings = totalHoldings.marketValue + customer.balance;
            SumOfCashHoldings.Text = string.Format("{0:C}", totalcashandholdings);
            gain = totalcashandholdings - customer.openBalance;
            string percentGainString = "";
            if (customer.openBalance != 0)
                percentGain = gain / customer.openBalance * 100;
            else
                percentGain = 0;
            if (gain > 0)
            {
                percentGainString = string.Format("{0:N}%" + Settings.UPARROWLINK, percentGain);
                Gain.ForeColor = System.Drawing.ColorTranslator.FromHtml("#43A12B");
                PercentGain.ForeColor = System.Drawing.ColorTranslator.FromHtml("#43A12B");
            }
            else
                if (gain < 0)
                {
                    percentGainString = string.Format("{0:N}%" + Settings.DOWNARROWLINK, percentGain);
                    Gain.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A40707");
                    PercentGain.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A40707");
                }
                else
                {
                    percentGainString = string.Format("{0:N}%", percentGain);
                }
            Gain.Text = string.Format("{0:C}", gain);
            PercentGain.Text = percentGainString;
        }

     

        private delegate TotalHoldingsUI GetHoldingsAsync();
        TotalHoldingsUI GetHoldings()
        {
            BSLClient businessServicesClient = new BSLClient();
            return businessServicesClient.getHoldings(userid);
        }

        private delegate AccountDataUI GetCustomerAsync();
        AccountDataUI GetCustomer()
        {
            BSLClient businessServicesClient = new BSLClient();
            return businessServicesClient.getAccountData(userid);
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
