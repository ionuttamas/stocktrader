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

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Description;
using Microsoft.ServiceBus.Messaging;

namespace Trade.OrderProcessorAsyncClient
{
    public class ServiceBusMessageInspector : BehaviorExtensionElement, IClientMessageInspector, IEndpointBehavior
    {
        private string _sender;

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            string sender = _sender;

            object incomingBrokeredMessageProperty = null;
            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageProperties.TryGetValue(BrokeredMessageProperty.Name, out incomingBrokeredMessageProperty))
            {
                object senderObject = null;
                if (((BrokeredMessageProperty)incomingBrokeredMessageProperty).Properties.TryGetValue("sender", out senderObject))
                {
                    sender = (string) senderObject;
                }
            }

            BrokeredMessageProperty brokeredMessageProperty = new BrokeredMessageProperty();
            brokeredMessageProperty.Properties.Add("sender", sender);

            request.Properties[BrokeredMessageProperty.Name] = brokeredMessageProperty;

            return Guid.NewGuid();
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            TransportClientEndpointBehavior transportClientEndpointBehavior = endpoint.Behaviors.Find<TransportClientEndpointBehavior>();
            if (transportClientEndpointBehavior != null)
            {
                SharedSecretCredential sharedSecret = transportClientEndpointBehavior.Credentials.SharedSecret;
                if (sharedSecret != null)
                {
                    clientRuntime.MessageInspectors.Add(new ServiceBusMessageInspector { _sender = sharedSecret.IssuerName });
                }
            }
        }

        protected override object CreateBehavior()
        {
            return new ServiceBusMessageInspector();
        }

        public override Type BehaviorType
        {
            get { return typeof(ServiceBusMessageInspector); }
        }
    }
}
