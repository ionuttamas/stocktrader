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

//  ITradeServiceFacadeWcf:  The Wcf/.NET 4.0 Web Service facade Interface for business service
//  operations in Trade.BusinessServices.TradeService.cs class. 


using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Trade.BusinessServiceDataContract;

namespace Trade.BusinessServiceContract

{
    //This begins the definition of the WCF Service Contract, a fundamental concept within WCF for
    //maintaining interfaces between services within a composite, while
    //hiding implementation details from service consumers. 
    //
    //Note the IBM namespace is used to allow the IBM Java WebSphere application, out-of-the-box, to 
    //have bi-directional interoperability with .NET StockTrader.  The IBM proxy expects this namespace, so our
    //service emits it (and hence our WCF client receives it).  Interoperability works 
    //without any code changes to the Java implementation; simply point the endpoint for Java JSP Web site
    //at the WCF Web Services (either self hosted or IIS-hosted). The converse also works-
    //point the ASP.NET StockTrader application at the backend Java services via the
    //AccessMode=WebSphere_WebService setting after setting the correct endpoint to
    //your Java server.


    /// <summary>
    /// This class is included for seamless interoperability with the existing WebSphere SOAP proxy for IBM WebSphere
    /// Java Trade 6.1. The IBM WebSphere SOAP Proxy uses method name in body, vs. method name in SOAP Action Header.  This code
    /// will properly dispatch methods based on an empty SOAP Action, instead routing based on the method name in the
    /// XML body.  For StockTrader 5, however, we are not using this behavior, although you can easily uncomment it below to
    /// implement if you wish.
    /// </summary>
    class DispatchByBodyElementOperationSelector : IDispatchOperationSelector
    {
        #region IDispatchOperationSelector Members
        Dictionary<XmlQualifiedName, string> dispatchDictionary;

        public DispatchByBodyElementOperationSelector(Dictionary<XmlQualifiedName, string> dispatchDictionary)
        {
            this.dispatchDictionary = dispatchDictionary;
        }



        private Message CreateMessageCopy(Message message, XmlDictionaryReader body)
        {
            Message copy = Message.CreateMessage(message.Version, message.Headers.Action, body);
            copy.Headers.CopyHeaderFrom(message, 0);
            copy.Properties.CopyProperties(message.Properties);
            return copy;
        }

        public string SelectOperation(ref System.ServiceModel.Channels.Message message)
        {
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();

            XmlQualifiedName lookupQName = new XmlQualifiedName(bodyReader.LocalName, bodyReader.NamespaceURI);
            message = CreateMessageCopy(message, bodyReader);
            if (dispatchDictionary.ContainsKey(lookupQName))
            {
                return dispatchDictionary[lookupQName];
            }
            else
            {
                return null;
            }
        }

        #endregion
    }

    /// <summary>
    /// This class is included for seamless interoperability with the existing WebSphere SOAP proxy for IBM WebSphere
    /// Trade 6.1. The IBM WebSphere SOAP Proxy uses method name in body, vs. method name in SOAP Action Header.  This code
    /// will dispatch methods based on an empty SOAP Action, instead routing based on the method name in the
    /// XML body.  This is not the the recommended approach, however, so for StockTrader 5 we keep the behavior definition but
    /// do not implement it by default.  Can uncomment it below if you wish.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    sealed class DispatchByBodyBehaviorAttribute : Attribute, IContractBehavior
    {
        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // no binding parameters need to be set here
            return;
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            // this is a dispatch-side behavior which doesn't require
            // any action on the client
            return;
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
            // We iterate over the operation descriptions in the contract and
            // record the QName of the request body child element and corresponding operation name
            // to the dictionary to be used for dispatch 
            Dictionary<XmlQualifiedName, string> dispatchDictionary = new Dictionary<XmlQualifiedName, string>();
            foreach (OperationDescription operationDescription in contractDescription.Operations)
            {
                XmlQualifiedName qname =
                    new XmlQualifiedName(operationDescription.Messages[0].Body.WrapperName, operationDescription.Messages[0].Body.WrapperNamespace);

                dispatchDictionary.Add(qname, operationDescription.Name);
            }

            // Lastly, we create and assign an instance of our operation selector that
            // gets the dispatch dictionary we've just created, unless the encoding is Binary over TCP (not XML).
            if (endpoint.Binding.Scheme=="net.tcp")
                return;
            dispatchRuntime.OperationSelector =
                new DispatchByBodyElementOperationSelector(dispatchDictionary);
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            // 
        }
        #endregion
    }

     
    /// <summary>
    /// This is the service contract for Trade Business Services. It defines the business service layer operations
    /// that are separately implemented in an implementation class.  Uncomment the DispatchByBodyBehavior below if
    /// required for interop.
    /// </summary>
    [ServiceContract(Namespace = "http://trade.samples.websphere.ibm.com")]
    // [DispatchByBodyBehavior]  
    public interface ITradeServices
    {
        [OperationContract(Action = "")]
        void emptyMethodAction();

        [OperationContract(Action="isOnline", Name="isOnline", IsOneWay=true)]
        void isOnline();

        [OperationContract()]
        [return: MessageParameter(Name = "loginReturn")]
        AccountDataModel login(string userID, string password);

        [OperationContract(Action = "getOrders")]
        [ServiceKnownType(typeof(OrderDataModel))]
        [return: MessageParameter(Name = "getOrdersReturn")]
        List<OrderDataModel> getOrders(string userID);

        [OperationContract(Action = "getAccountData")]
        [return: MessageParameter(Name = "getAccountDataReturn")]
        AccountDataModel getAccountData(string userID);

        [OperationContract(Action = "getAccountProfileData")]
        [return: MessageParameter(Name = "getAccountProfileDataReturn")]
        AccountProfileDataModel getAccountProfileData(string userID);

        [OperationContract(Action = "updateAccountProfile")]
        [return: MessageParameter(Name = "updateAccountProfileReturn")]
        AccountProfileDataModel updateAccountProfile(AccountProfileDataModel profileData);

        [OperationContract(Action = "logout")]
        [return: MessageParameter(Name = "logoutReturn")]
        void logout(string userID);

        [OperationContract(Action = "buy")]
        [return: MessageParameter(Name = "buyReturn")]
        OrderDataModel buy(string userID, string symbol, double quantity, int orderProcessingMode);

        [OperationContract(Action = "sell")]
        [return: MessageParameter(Name = "sellReturn")]
        OrderDataModel sell(string userID, string holdingID, int orderProcessingMode);

        [OperationContract(Action = "getHoldings")]
        [ServiceKnownType(typeof(HoldingDataModel))]
        [return: MessageParameter(Name = "getHoldingsReturn")]
        List<HoldingDataModel> getHoldings(string userID);

        [OperationContract(Action = "register")]
        [return: MessageParameter(Name = "registerReturn")]
        AccountDataModel register(string userID, string password, string fullname, string address, string email, string creditcard, decimal openBalance);

        [OperationContract(Action = "registerDevice")]
        [return: MessageParameter(Name = "registerDeviceReturn")]
        DeviceDataModel registerDevice(string userID, string deviceID);

        [OperationContract(Action = "getClosedOrders")]
        [ServiceKnownType(typeof(OrderDataModel))]
        [return: MessageParameter(Name = "getClosedOrdersReturn")]
        List<OrderDataModel> getClosedOrders(string userID);

        [OperationContract(Action = "getMarketSummary")]
        [ServiceKnownType(typeof(QuoteDataModel))]
        [return: MessageParameter(Name = "getMarketSummaryReturn")]
        MarketSummaryDataModelWS getMarketSummary();

        [OperationContract(Name = "getQuote")]
        [return: MessageParameter(Name = "getQuoteReturn")]
        QuoteDataModel getQuote(string symbol);

        [OperationContract(Name = "getQuotes")]
        [return: MessageParameter(Name = "getQuotesReturn")]
        List<QuoteDataModel> getQuotes(string symbols);

        [OperationContract(Action = "getHolding")]
        [return: MessageParameter(Name = "getHoldingReturn")]
        HoldingDataModel getHolding(string userID, string holdingID);

        [OperationContract(Action = "getTopOrders")]
        [ServiceKnownType(typeof(OrderDataModel))]
        [return: MessageParameter(Name = "getTopOrdersReturn")]
        List<OrderDataModel> getTopOrders(string userID);

        [OperationContract(Action = "sellEnhanced")]
        [return: MessageParameter(Name = "sellEnhancedReturn")]
        OrderDataModel sellEnhanced(string userID, string holdingID, double quantity);
    }
}
