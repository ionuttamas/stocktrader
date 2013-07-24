using System;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Trade.AzureNewsPublisher.Properties;
using Trade.AzureNewsPublisherDataContracts;

namespace Trade.AzureNewsPublisher
{
    class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Title = "Lorem ipsum dolor sit amet";
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur metus augue, pellentesque et adipiscing sed, bibendum ut risus. Vestibulum laoreet risus vel tortor fermentum luctus.";
            Url = "http://www.msn.com";
            Submit = new GenericCommand(x => CanSubmitNews, x => SubmitNews());
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public ICommand Submit { get; private set; }

        private bool CanSubmitNews
        {
            get { return !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Url); }
        }

        private void SubmitNews()
        {
            try
            {
                var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", Settings.Default.Namespace, string.Empty);
                var tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(Settings.Default.ManagementName, Settings.Default.ManagementKey);

                var namespaceManager = new NamespaceManager(serviceUri, tokenProvider);
                var topics = namespaceManager.GetTopics().Where(x => x.Path.StartsWith(Settings.Default.Topic));

                var messagingFactory = MessagingFactory.Create(serviceUri, tokenProvider);
                foreach (var topic in topics)
                {
                    var topicClient = messagingFactory.CreateTopicClient(topic.Path);
                    topicClient.Send(new BrokeredMessage(new NewsItem
                                                             {
                                                                 Description = Description,
                                                                 Title = Title,
                                                                 Url = Url
                                                             }, new DataContractJsonSerializer(typeof (NewsItem)))
                                         {
                                             TimeToLive = TimeSpan.FromMinutes(10)
                                         });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when trying to submit the news item: " + ex);
            }
        }

        class GenericCommand : ICommand
        {
            private readonly Func<object, bool> _canExecute;
            private readonly Action<object> _execute;

            public GenericCommand(Func<object, bool> canExecute, Action<object> execute)
            {
                _canExecute = canExecute;
                _execute = execute;
            }

            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    _execute(parameter);
                }
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute(parameter);
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
