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
//  Note that for ClosedOrders alert control we use in-page script to generate our repeating table rows in the HTML. 
//  vs. using databound Repeater controls as we do in the AccountOrders (control), Portfolio (page), and MarketSummary (control).
//  The choice of display method is up to the architect; Repeaters and GridViews have many features you do not
//  get with in-page script.

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;

namespace Trade.Web
{
    /// <summary>
    /// Checks for closed orders, and displays alert if any. 
    /// </summary>
    public partial class ClosedOrders : System.Web.UI.UserControl
    {
        public List<OrderDataUI> closedOrderData;

        protected override void OnLoad(EventArgs e)
        {
            
            
            //Here we are going to use an absolute expiration and the .NET object cache to enable
            //the application to optionally not execute the order alert query on every page; rather just check every
            //n seconds (this setting is adjustable in Web.config).  Executing the alert control 
            //logic on every page is not the best design: users can stand to get their completion alerts after 60
            //seconds vs. right away, plus we are going to invalidate the cache entry
            //anyway on an order being placed, so they will only have to wait if they place the order from
            //another browser. The reduction on database queries is substantial (and impact on perf) 
            //simply by making this choice. They will STILL get an order alert within 60 seconds of
            //browsing even if another program (such as the async Order Processor Service or any program) completes 
            //the order outside of the scope of the Web application.

            //Order alerts and stock market summary/mkt index are the only two places in this app that really
            //make sense to cache.  All other data elements such as account info/balances, stock prices,
            //holdings, orders etc. are not good candidates for caching in our opinion.  These data elements 
            //should always reflect what is actually in the database.  This is becuase other systems besides 
            //StockTrader would quite likely be changing this data in the real world, so invalidating the cache within StockTrader app 
            //(ala WebSphere Trade 6.1) does more harm than good--since the middle tier is completely unaware of
            //what other applications may have done to change the database information being cached.
            //On the other hand, order alerts and market summaries are excellent choices for caching:
            //data here can be safely be refreshed every 30, 60 seconds (or more) without impacting data 
            //integrity or alarming a user with an inconsistent value. 

            //The .NET cache for this control will only be used if the Web.config setting
            //"CheckOrderAlertsOnEveryRequest" = false. For benchmark comparisons, its important this be true
            //if measuring .NET perf against WebSphere or other product if those products are not also caching data.
            //In our published data, we used the "true" setting so the control is executed on every
            //requested page, and alerts come up immediately as opposed to slightly delayed.   

            
            string userid = HttpContext.Current.User.Identity.Name;
            if (userid == "")
            {
                HttpCookie authcookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authcookie != null)
                {
                    FormsAuthenticationTicket ticket = (FormsAuthenticationTicket)FormsAuthentication.Decrypt(authcookie.Value);
                    if (ticket != null)
                        userid = ticket.Name;
                }
            }
            string item = (string)Cache[Settings.CACHE_KEY_CLOSED_ORDERSALERT];
            if ((userid != "" && userid != null && Settings.CHECK_ORDER_ALERT_EVERY_REQUEST) || item == null)
            {
                //Either the setting in web.config is set for checking on every page, or
                //the timeout on our cache has expired. So we must invoke our BSL layer
                //now to check for closed orders.
                BSLClient businessServicesClient = new BSLClient();
                closedOrderData = businessServicesClient.getClosedOrders(userid);
                //We are not interested in actually caching any data here: after all, users only get notified
                //via an alert 1 time per order.  Rather, we are using the cache as a convenient way to
                //ensure alert checks only happen based on our desired frequency.  This frequency is set as a config setting.
                if (!Settings.CHECK_ORDER_ALERT_EVERY_REQUEST)
                    Cache.Insert(Settings.CACHE_KEY_CLOSED_ORDERSALERT, "O", null, System.DateTime.UtcNow.AddSeconds(Settings.ORDER_ALERT_CHECK_FREQUENCY), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }

    }
}
