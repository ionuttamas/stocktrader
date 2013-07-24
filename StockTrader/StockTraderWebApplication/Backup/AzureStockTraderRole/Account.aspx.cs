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
using System.Web.UI;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;
using Trade.Utility;

namespace Trade.Web
{
    /// <summary>
    /// Displays the Account Summary information by querying Business Services.
    /// </summary>
    public partial class Account : System.Web.UI.Page
    {
        AccountProfileDataUI customerprofile;
        AccountDataUI customer;
        string userid;
        string action;
        GetCustomerProfileAsync caller1;
        GetCustomerAsync caller2;
        IAsyncResult result1;
        IAsyncResult result2;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultFocus = FullName.ClientID;
            if (!(Request["action"] == null) && !Input.InputText(Request["action"], StockTraderUtility.EXPRESSION_NAME_13))
                logout();
            userid = HttpContext.Current.User.Identity.Name;
            
            if (userid == null)
                logout();
            action = Request["action"];
            if (!IsPostBack)
            {
                // Create the delegate.
                caller1 = new GetCustomerProfileAsync(GetCustomerProfile);
                // Initiate the asychronous call.
                result1 = caller1.BeginInvoke(null,null);
                // Repeat
                caller2 = new GetCustomerAsync(GetCustomer);
                // Initiate the asychronous call.
                result2 = caller2.BeginInvoke(null, null);
            }
            else
            {
                customerprofile = (AccountProfileDataUI)ViewState["custprofile"];
                customer = (AccountDataUI)ViewState["cust"];
            }
        }

        private delegate AccountProfileDataUI GetCustomerProfileAsync();
        AccountProfileDataUI GetCustomerProfile()
        {
            
            BSLClient businessServicesClient = new BSLClient();
            return businessServicesClient.getAccountProfileData(userid);
        }

        private delegate AccountDataUI GetCustomerAsync();
        AccountDataUI GetCustomer()
        {
           
            BSLClient businessServicesClient = new BSLClient();
            return businessServicesClient.getAccountData(userid);
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Call EndInvoke to wait for the asynchronous calls to complete
                customerprofile = caller1.EndInvoke(result1);
                customer = caller2.EndInvoke(result2);
                ViewState["custprofile"] = customerprofile;
                ViewState["cust"] = customer;
                UpdateMessage.Text = "Account: " + customerprofile.userID;
            }

            NumOrdersShown.Text = AccountOrdersControl.totalOrders.ToString();
            if (AccountOrdersControl.ordersRequested == Settings.MAX_DISPLAY_ORDERS)
            {
                orderLink.Text = "Show Top <b>" + Settings.MAX_DISPLAY_TOP_ORDERS.ToString() + "</b> Orders";
                orderLink.PostBackUrl = Settings.PAGE_ACCOUNT + "?action=showtoporders";
            }
            else
            {
                orderLink.Text = "Show Top <b>" + Settings.MAX_DISPLAY_ORDERS.ToString() + "</b> Orders";
                orderLink.PostBackUrl = Settings.PAGE_ACCOUNT;
            }

            Name.Text = customer.profileID;
            AccountID.Text = customer.accountID.ToString();
            CreationDate.Text = customer.creationDate.ToString("f");
            LoginCount.Text = customer.loginCount.ToString();
            OpenBalance.Text = string.Format("{0:C}", customer.openBalance);
            if (customer.balance > 0)
                Balance.ForeColor = System.Drawing.ColorTranslator.FromHtml("#43A12B");
            else
                Balance.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A40707");
            Balance.Text = string.Format("{0:C}", customer.balance);
            TotalLogout.Text = customer.logoutCount.ToString();
            LastLogin.Text = customer.lastLogin.ToString("f");
            Email.Text = customerprofile.email;
            FullName.Text = customerprofile.fullName;
            Address.Text = customerprofile.address;
            CreditCard.Text = customerprofile.creditCard;
        }
        
        private void submitData(AccountProfileDataUI customerprofile)
        {
            if (userid.ToLower().Equals("demouser"))
            {
                UpdateMessage.Text = "Demouser cannot be changed.";
                return;
            }
            Page.Validate();
            if (Page.IsValid)
            {

                customerprofile.address = Address.Text;
                customerprofile.creditCard = CreditCard.Text;
                customerprofile.email = Email.Text;
                customerprofile.fullName = FullName.Text;
                customerprofile.password = Password.Text;
                BSLClient businessServicesClient = new BSLClient();
                customerprofile = businessServicesClient.updateAccountProfile(customerprofile);
                ViewState["custprofile"] = customerprofile;
                UpdateMessage.Text = "Account Updated";
            }
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

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            string userid = HttpContext.Current.User.Identity.Name;
            if (userid == null)
                logout();
            customerprofile = (AccountProfileDataUI)ViewState["custprofile"];
            customer = (AccountDataUI)ViewState["cust"];
            submitData(customerprofile);
        }
}
}
