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
using System.Globalization;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.AccessControlExtensions;
using Microsoft.ServiceBus.AccessControlExtensions.AccessControlManagement;
using Microsoft.ServiceBus.Messaging;

namespace Trade.BusinessServiceImplementation
{
    /// <summary>
    /// This class is used to interact with ACS/Service Bus.
    /// </summary>
    class ServiceBusHelper
    {
        private readonly string _serviceNamespace;
        private readonly string _managementSecret;
        private readonly AccessControlSettings _accessControlSettings;
        private readonly Uri _serviceUri;
        private readonly NamespaceManager _namespaceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusHelper"/> class.
        /// </summary>
        /// <param name="serviceNamespace">The service namespace (i.e. without .servicebus.windows.net).</param>
        /// <param name="managementSecret">The management secret.</param>
        public ServiceBusHelper(string serviceNamespace, string managementSecret)
        {
            _serviceNamespace = serviceNamespace;
            _managementSecret = managementSecret;
            _accessControlSettings = new AccessControlSettings(_serviceNamespace, _managementSecret);
            _serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", _serviceNamespace, string.Empty);
            _namespaceManager = new NamespaceManager(_serviceUri, TokenProvider.CreateSharedSecretTokenProvider("owner", _managementSecret));
        }

        /// <summary>
        /// Creates a user in the Service Bus ACS namespace.
        /// </summary>
        /// <param name="name">The user's name.</param>
        /// <param name="description">The user's description.</param>
        /// <returns>The user password.</returns>
        public string CreateUser(string name, string description)
        {
            string key = null;

            ManagementService managementServiceClient = ManagementServiceHelper.CreateManagementServiceClient(_accessControlSettings);
            var identity = managementServiceClient.GetServiceIdentityByName(name);
            if (identity == null)
            {
                // Create the identity
                AccessControlServiceIdentity acsIdentity = AccessControlServiceIdentity.Create(_accessControlSettings, name);
                acsIdentity.Save();
                // Setting a property - description - needs to be done as part of a separate call
                acsIdentity.Description = description;
                acsIdentity.Save();

                key = acsIdentity.GetKeyAsBase64();
            }
            else
            {
                key = Convert.ToBase64String(identity.ServiceIdentityKeys.First(x => x.Type == "Symmetric").Value);
            }

            return key;
        }

        /// <summary>
        /// Creates a topic in the SB namespace.
        /// </summary>
        /// <param name="topicName">Name of the topic.</param>
        public void CreateTopic(string topicName)
        {
            if (!_namespaceManager.TopicExists(topicName))
            {
                try
                {
                    _namespaceManager.CreateTopic(topicName);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    // May occur if two threads try and create the topic at the same time.
                }
            }
        }

        /// <summary>
        /// Creates a subscription to a given topic.
        /// </summary>
        /// <param name="topicName">Name of the topic for the subscription.</param>
        /// <param name="subscriptionName">Name of the subscription.</param>
        public void CreateSubscription(string topicName, string subscriptionName)
        {
            if (!_namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                _namespaceManager.CreateSubscription(topicName, subscriptionName);
            }
        }

        /// <summary>
        /// Grant a user LISTEN permissions for a given subscription.
        /// </summary>
        /// <param name="topicName">Name of the topic.</param>
        /// <param name="subscriptionName">Name of the subscription.</param>
        /// <param name="userName">Name of the user.</param>
        public void CreateRuleForUser(string topicName, string subscriptionName, string userName)
        {
            AccessControlList acsList = NamespaceAccessControl.GetAccessControlList(GetSubscriptionUri(topicName, subscriptionName), _accessControlSettings);
            if (!acsList.Any(x => x.Right == ServiceBusRight.Listen && x.Condition.ClaimValue == userName))
            {
                acsList.AddRule(IdentityReference.CreateServiceIdentityReference(userName), ServiceBusRight.Listen);
                acsList.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the URI for a given subscription.
        /// </summary>
        /// <param name="topicName">Name of the topic.</param>
        /// <param name="name">Name of the subscription.</param>
        /// <returns></returns>
        public Uri GetSubscriptionUri(string topicName, string name)
        {
            string subscriptionRelativePath = string.Format(CultureInfo.InvariantCulture, "/{0}/Subscriptions/{1}/", topicName, name);
            return ServiceBusEnvironment.CreateServiceUri("https", _serviceNamespace, subscriptionRelativePath);
        }
    }
}
