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
using System.Text;
using System.Web;
using System.Web.Security;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;
using Trade.Utility;

namespace Trade.Web
{
    /// <summary>
    /// Allows users to enter number of shares for a buy/sell operation.
    /// </summary>
    public partial class StockTrade : System.Web.UI.Page
    {
        string tradenow = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultFocus = quantity.ClientID;
           // if (TextBoxID.Text == "Refresh")
           //     Response.Redirect(Settings.PAGE_HOME, true);
            string userid = HttpContext.Current.User.Identity.Name;
            if (userid == null)
                logout();
            if (!IsPostBack)
            {
                PanelTrade.Visible = false;
                PanelConfirm.Visible = true;
                BSLClient businessServicesClient = new BSLClient();
                string action = Request["action"];
                if (!Input.InputText(action, StockTraderUtility.EXPRESSION_NAME_10))
                    Response.Redirect(Settings.PAGE_HOME, true);
                if (action == StockTraderUtility.ORDER_TYPE_BUY)
                {
                    string quoteSymbol = Request["symbol"];
                    if (!Input.InputText(quoteSymbol, StockTraderUtility.EXPRESSION_QUOTE_ID))
                        Response.Redirect(Settings.PAGE_HOME, true);
                    QuoteDataUI quote = businessServicesClient.getQuote(quoteSymbol);
                    if (quote == null)
                        Response.Redirect(Settings.PAGE_HOME, true);
                    TradeOperation.Text = "<span style=\"text-align:center\">You have requested to <b>buy</b> shares of " + quote.quoteLink + " which is currently trading at " + quote.priceWithArrow + "</span>";
                    TextBoxID.Text = quote.symbol;
                }
                else
                {
                    string holdingID = Request["holdingid"];
                    if (!Input.InputText(holdingID, StockTraderUtility.EXPRESSION_HOLDINGID))
                        Response.Redirect(Settings.PAGE_HOME, true);
                    if (action == StockTraderUtility.ORDER_TYPE_SELL)
                    {
                        HoldingDataUI holding = businessServicesClient.getHolding(userid, holdingID);
                        if (holding == null)
                            Response.Redirect(Settings.PAGE_HOME, true);
                        StringBuilder strBldr = new StringBuilder("You have requested to sell all or part of your holding ");
                        strBldr.Append(holdingID);
                        strBldr.Append(". This holding has a total of ");
                        strBldr.Append(holding.quantity);
                        strBldr.Append(" shares of stock <b>");
                        strBldr.Append(holding.quoteID);
                        strBldr.Append("</b>. Please indicate how many shares to sell.");
                        TradeOperation.Text = strBldr.ToString();
                        decimal amount = Convert.ToDecimal(holding.quantity);
                        quantity.Text = amount.ToString();
                        ButtonTrade.Text = "  Sell  ";
                        TextBoxID.Text = holdingID;
                    }
                }
            }
            else
            {
                PanelTrade.Visible = true;
                PanelConfirm.Visible = false;
            }
        }

        private void logout()
        {
            FormsAuthentication.SignOut();
            Response.Redirect(Settings.PAGE_LOGIN, true);
        }

        protected void ButtonTrade_Click(object sender, EventArgs e)
        {
            double quantityTrade = 0;
            string symbol = null;
            string holdingID = null;
            string userid = HttpContext.Current.User.Identity.Name;
            if (userid == null)
                logout();
            if (!double.TryParse(quantity.Text, out quantityTrade))
            {
                RequiredFieldValidator1.IsValid=false;
                RequiredFieldValidator1.ErrorMessage = "Please enter a valid value...";
                return;
            }
            BSLClient businessServicesClient = new BSLClient();
            OrderDataUI order = null;
            string action = Request["action"];
            if (!Input.InputText(action, StockTraderUtility.EXPRESSION_NAME_10))
                Response.Redirect(Settings.PAGE_HOME, true);
            
            if (tradenow == "sell")
            {
                ButtonTrade.Text = "Sell";
                holdingID = Request["holdingid"];
            }
            else if (tradenow == "buy")
            {
                ButtonTrade.Text = "Buy";
                symbol = (string)Request["symbol"];
            }
            ConfirmMessage.Text = "";
            try
            {
                if (ButtonTrade.Text.Contains("Buy"))
                {
                    if (symbol == null)
                        symbol = TextBoxID.Text;
                    order = businessServicesClient.buy(userid, symbol, quantityTrade);
                }
                else if (ButtonTrade.Text.Contains("Sell"))
                {
                    if (holdingID == null)
                        holdingID = TextBoxID.Text;

                    if (quantityTrade == -1)
                        quantityTrade = 0;  //Value of 0 indicates to sell entire holding.
                    order = businessServicesClient.sell(userid, holdingID, quantityTrade);
                }
                else
                    //Goodbye! Only valid ops are buy and sell. This is a harsh 
                    //penalty for trying to be tricky.
                    Response.Redirect(Settings.PAGE_LOGOUT, true);
            }
            catch (System.ServiceModel.FaultException)
            {
                ConfirmMessage.Text = "<span style=\"color:Maroon;\">" + StockTraderUtility.EXCEPTION_MESSAGE_REMOTE_OPS_EXCEPTION + "</span>";
            }
            catch (Exception)
            {
                //Two catch blocks perhaps in future could customize the message a bit more depending on exception type.  
                ConfirmMessage.Text = "<span style=\"color:Maroon;\">" + StockTraderUtility.EXCEPTION_MESSAGE_REMOTE_OPS_EXCEPTION + "</span>";
            }
            if (order != null)
            {
                string orderIdStr = order.orderID.ToString();
                OrderID.Text = orderIdStr;
                OrderStatus.Text = order.orderStatus;
                OpenDate.Text = order.openDate.ToString();
                CompletionDate.Text = order.completionDate.ToString();
                OrderFee.Text = string.Format("{0:C}", order.orderFee);
                OrderType.Text = order.orderType;
                string orderLink = order.quoteLink;
                Symbol.Text = orderLink;
                string orderQty = string.Format("{0:0,0}", order.quantity);
                QtyTraded.Text = orderQty;
                StringBuilder strBuilder = new StringBuilder("Order <b>");
                strBuilder.Append(orderIdStr);
                strBuilder.Append("</b> to ");
                strBuilder.Append(order.orderType);
                strBuilder.Append(" ");
                strBuilder.Append(orderQty);
                strBuilder.Append(" shares of ");
                strBuilder.Append(orderLink);
                strBuilder.Append(" has been submitted for processing.<br/><br/>");
                strBuilder.Append("Order Details:");
                ConfirmMessage.Text = (strBuilder.ToString());
            }
            else
                if (ConfirmMessage.Text=="")
                    ConfirmMessage.Text = StockTraderUtility.EXCEPTION_MESSAGE_BAD_ORDER_RETURN;
            TextBoxID.Text = "";
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Settings.PAGE_HOME, true);
        }

        protected void QuoteButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(Settings.PAGE_QUOTES + "?symbols=" + symbols.Text, true);
        }
    }
}