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
// A sample custom error handler.  Used by the Order Processor Service, as an example.  In this case,
// simply writing the exception information into the WindowsConsoleHost for display in realtime.
//======================================================================================================


using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using ConfigService.ServiceConfigurationUtility;

namespace Trade.OrderProcessorImplementation
{
    public class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        #region ErrorBehaviorAttribute Members
        ErrorHandler ErrorHandler;

        public ErrorBehaviorAttribute()
        {
            this.ErrorHandler = new Trade.OrderProcessorImplementation.ErrorHandler();
        }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(ErrorHandler);
            }
        }
        #endregion
    }

    public class ErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // no-op -we are not interested in this.
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="error">Exception thrown.</param>
        /// <returns></returns>
        public bool HandleError(Exception error)
        {
            //Note these first are idle timeouts based on the tcp binding setting receiveTimeout.  They will always happen
            //when a tcp connection is idle beyond the timeout value.  They are expected, and hence not logged.  
            //Also, Config Service clients automatically re-establish tcp connections if broken by the host.
            if (error.Message.Contains("receive timeout being exceeded by the remote host"))
                return true;
            //OK we have a real exception.  Log it!
            ConfigUtility.writeErrorConsoleMessage("\nError! Exception is: " + error.ToString(), EventLogEntryType.Error, true, new Trade.OrderProcessorServiceConfigurationSettings.Settings());
            return true;
        }
    }
}