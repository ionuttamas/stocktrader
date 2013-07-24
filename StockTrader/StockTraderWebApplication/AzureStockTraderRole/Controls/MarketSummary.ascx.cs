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
using Trade.StockTraderWebApplicationModelClasses;
using Trade.StockTraderWebApplicationServiceClient;
using Trade.StockTraderWebApplicationSettings;

namespace Trade.Web
{
    /// <summary>
    /// Displays the Market Summary tables using repeaters. Used on TradeHome.aspx.  This control should
    /// implement output caching as the query for determining stock index is somewhat expensive.  
    /// A good use of caching of read-only data, where user can stand to be seeing data 60 seconds old.
    /// </summary>
    public partial class MarketSummary : System.Web.UI.UserControl
    {
        public MarketSummaryDataUI marketSummaryData = null;

        protected override void OnLoad(EventArgs e)
        {
            BSLClient businessServicesClient = new BSLClient();
            marketSummaryData = businessServicesClient.getMarketSummary();
            List<QuoteDataUI> topGainers = marketSummaryData.topGainers;
            List<QuoteDataUI> topLosers = marketSummaryData.topLosers;
            summaryDate.Text = DateTime.Now.ToString("f");
            TSIA.Text = String.Format("{0:N}",marketSummaryData.TSIA);
            decimal gainpercent =  marketSummaryData.gainPercent;
            if (gainpercent > 0)
            {
                GainPercent.ForeColor = System.Drawing.ColorTranslator.FromHtml("palegreen");  
                GainPercent.Text = String.Format("{0:N}" + Settings.UPARROWLINK, gainpercent);
            }
                else if (gainpercent < 0)
                {
                    GainPercent.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A40707");
                    GainPercent.Text = String.Format("{0:N}" + Settings.DOWNARROWLINK, gainpercent);
                }
                    else
                    {
                        GainPercent.Text = String.Format("{0:N}", gainpercent);
                    }
            Volume.Text = String.Format("{0:N}", marketSummaryData.volume);
            TopGainers.DataSource = marketSummaryData.topGainers;
            TopLosers.DataSource = marketSummaryData.topLosers;
            TopGainers.DataBind();
            TopLosers.DataBind();
        }
    }
}
