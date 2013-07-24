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

//======================================================================================================
// The OrderProcessor ConfigurationService implementation class.
//======================================================================================================


using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

using ConfigService.ServiceConfiguration.DataContract;
using ConfigService.ServiceNodeCommunication.DataContract;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceConfigurationUtility;
using ConfigService.ServiceConfigurationBase;
using ConfigService.ServiceConfigurationContract;
using Trade.OrderProcessorServiceConfigurationSettings;
using Trade.OrderProcessorContract;
using Trade.Utility;

namespace Trade.OrderProcessorHostConfigurationImplementation
{
    /// <summary>
    /// This is the Configuration Service implementation for the Order Processor Service Host. It performs the
    /// operations for service/app configuration.  Note this class now inherits from a base implementation.
    /// You can override any/all methods in the Configuration Service Contract to provide custom implementations, however.
    /// In this case, we do not need to, as will be the case for most other custom apps/services. 
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class ConfigurationService : ConfigurationServiceBase
    {
        /// <summary>
        /// Simple constructor.  Make sure to instantiate the settings instance class, which is defined in the base class.
        /// </summary>
        public ConfigurationService()
        {
            settingsInstance = new Settings();
        }
     }
 }

