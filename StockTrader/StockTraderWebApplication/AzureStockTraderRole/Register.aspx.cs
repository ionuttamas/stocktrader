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
using System.Web.Security;
using System.Web.UI;
using ConfigService.ServiceConfigurationUtility;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;
using Trade.Utility;

namespace Trade.Web
{
    /// <summary>
    /// Registers a new user, performs login/authentication via Business Services.
    /// </summary>
    public partial class Register : System.Web.UI.Page
    {
        string theuserID;
        string theFullName;
        string theAddress;
        string emailAddress;
        string thecreditCard;
        string thePassword;
        decimal theOpenBalance;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultFocus = UserID.ClientID;
            if (IsPostBack)
            {
                submitData();
            }
        }

        private void submitData()
        {
            Page.Validate();
            if (Page.IsValid)
            {
                RegisterMessage.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A40707");
                theFullName = FullName.Text;
                theAddress = Address.Text;
                emailAddress = Email.Text;
                thecreditCard = CreditCard.Text;
                theuserID = UserID.Text;
                thePassword = Password.Text;
                theOpenBalance = Decimal.Parse(OpenBalance.Text);
                AccountDataUI customer = null;
                RegisterMessage.Text = "";
                try
                {
                    BSLClient businessServicesClient = new BSLClient();
                    customer = businessServicesClient.register(theuserID, thePassword, theFullName, theAddress, emailAddress, thecreditCard, theOpenBalance);
                }
                catch (System.ServiceModel.FaultException)
                {
                    //TODO:  Need to use BSL Fault handler to provide fault detail so we can determine if this is real cause of exception.
                    RegisterMessage.Text = StockTraderUtility.EXCEPTION_MESSAGE_DUPLICATE_PRIMARY_KEY;
                }
                catch (Exception e)
                {
                    //Depending on web.config user setting just catch the duplicate key exception and display a 
                    //user-friendly message, or throw a 500 back to browser to make it easy to catch
                    //this error from benchmark scripts. You can mark DisplayDuplicateKeyExceptions true/false
                    //in Web.Config for this setting.  However, only applies when running in AccessMode = InProcess Activation.
                    //else we will get a ServiceModel fault exception (remote) per above catch block.  TODO above is improve handling
                    //of remote dupe/key exception from BSL to distinguish vs. a more catastrophic exception--like BSL is offline completely. 
                    if (e.Message.Contains(StockTraderUtility.EXCEPTION_DOTNET_DUPLICATE_PRIMARY_KEY) || e.Message.ToLower().Contains(StockTraderUtility.EXCEPTION_WEBSPHERE_DUPLICATE_PRIMARY_KEY.ToLower()))
                    {
                        if (Settings.DISPLAY_DUPLICATE_KEY_EXCEPTIONS)
                            throw;
                        RegisterMessage.Text = StockTraderUtility.EXCEPTION_MESSAGE_DUPLICATE_PRIMARY_KEY;
                    }
                    else
                        if (e.Message.Equals(ConfigSettings.EXCEPTION_SERVICEHOST_NOTREACHABLE))
                            RegisterMessage.Text = Utility.StockTraderUtility.EXCEPTION_MESSAGE_REMOTE_BSL_OFFLINE_EXCEPTION;
                        else
                            RegisterMessage.Text = "We are sorry, the StockTrader application may be down for maintenance, or is experiencing a technical issue. Please try again later.";
                }
                if (RegisterMessage.Text == "")
                {
                    FormsAuthentication.SetAuthCookie(customer.profileID, false);
                    Response.Redirect(Settings.PAGE_HOME, true);
                }
            }
        }
    }
}