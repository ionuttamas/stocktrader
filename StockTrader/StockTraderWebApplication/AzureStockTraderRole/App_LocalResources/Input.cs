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
using System.Text.RegularExpressions;

    /// <summary>
    /// This class trims input characters from a string, based on specified length.  Helps to prevent
    /// buffer overrun attacks.  Could be augmented with strip-out of script-injection attack characters, 
    /// although ASP.NET automatically does this for common special characters.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// This method uses regex to do basic input validation.</summary>
        /// <param name="inputString">String to check</param> 
        /// <param name="expression">regex expression</param> 
        public static bool InputText(string inputString, string expression)
        {
            // check incoming parameters for null or blank string
            if ((inputString != null) && (inputString != String.Empty))
            {
                return Regex.IsMatch(inputString, expression);
            }
            else
                return false;
        }
    }
