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
using System.Data.SqlClient;
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
    /// Performs authenticated login and sets FormsAuth cookie if user is authenticated against registered users DB.
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            Page.Form.DefaultFocus = uid.ClientID;
            Page.Form.DefaultButton = LoginButton.UniqueID;
            if (IsPostBack)
                processLogin();
        }

        protected void processLogin()
        {
            Page.Validate();
            if (Page.IsValid)
            {
                //Authenticate the user against Trade AccountProfile Table.
                //Notes on security: 
                //We are using ASP.NET Forms authentication, which automates the authentication process against
                //either a simple list of valid users, a backend database of registered users (StockTrader uses this mechanism),
                //Windows Active Directory, or any pluggable mechanism based on the extensibility of Forms Authentication in ASP.NET 4.0.
                //Via Forms Authentication, ASP.NET provides automatic authentication for restricted pages, and automates redirects to login forms 
                //such as this one. ASP.NET Forms authentication defaults to use SHA1 for HMAC Generation and AES for
                //Encryption, which is recommended. The key to securing .NET StockTrader is to use "Protection="All"
                //for the forms authentication directive in web.config, and just as importantly, an application such as this in production
                //would be run over SSL for all authenticated pages identified as restricted via ASP.NET Forms Authentication.  The Azure StockTrader,
                //for example, runs over SSL. Information on ASP.NET Forms Authentication, encryption and using Forms Authentication with SSL is available at:
                //                http://msdn.microsoft.com/en-us/library/xdt4thhy.aspx
                
                string userID = uid.Text;
                string password = txtpassword.Text;
                InValid.Text = "";
                AccountDataUI customer = null;
                try
                {
                    BSLClient businessServicesClient = new BSLClient();
                    customer = businessServicesClient.login(userID, password);
                }

                //There are several cases of exceptions here. Some depend on whether running BSL
                //In-process vs. remote activation; also, some Java interop cases need to be treated differently.
                catch (SqlException eSQL)
                {
                    if (eSQL.ErrorCode.Equals(StockTraderUtility.SQL_ACCESS_DENIED_OR_DB_NOT_FOUND))
                    {
                        InValid.Text = StockTraderUtility.EXCEPTION_MESSAGE_SQLSERVER_ADONET_PERMISSION_OR_NOT_FOUND_EXCEPTION;
                    }
                }
                //We will have one of these when BSL is online (passes ConfigService channel check), but something else
                //is wrong within BSL.  For example, it can't access the stocktrader db for any reason (including permissions issue, etc.)
                catch (System.ServiceModel.FaultException)
                {
                    InValid.Text = StockTraderUtility.EXCEPTION_MESSAGE_REMOTE_BSL_EXCEPTION;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains(StockTraderUtility.EXCEPTION_WEBSPHERE_USERID_NOTFOUND))
                        InValid.Text = StockTraderUtility.EXCEPTION_MESSAGE_INVALID_LOGIN;
                    else
                        if (e.Message.Equals(ConfigSettings.EXCEPTION_SERVICEHOST_NOTREACHABLE))
                            InValid.Text = Utility.StockTraderUtility.EXCEPTION_MESSAGE_REMOTE_BSL_OFFLINE_EXCEPTION;
                        else
                            InValid.Text = "A processing error ocurred.  The site may be undergoing maintenance, so try again later!";
                    customer = null;
                }
                if (customer == null)
                {
                    if (InValid.Text=="")
                        InValid.Text = StockTraderUtility.EXCEPTION_MESSAGE_INVALID_LOGIN;
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(customer.profileID, false);
                    Trade.StockTraderWebApplicationSettings.Settings.invokeCount++;
                    Response.Redirect(Settings.PAGE_HOME, true);
                }
            }
        }
    }
}
