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
//

//======================================================================================================
// The BSL ConfigurationService implementation class.  Note the StockTrader BSL only has to override 
// methods based on the fact it can run in the OPS in a special in-process mode.
// Typically, no overrides are needed, base class works fine as is for strictly remote services. SO
// PLEASE SEE THE CONFIG SERVICE 5 TEMPLATE GENERATED SERVICES AS NONE OF THE CUSTOM CODE BELOW IS 
// NECESSARY FOR TYPICAL SERVICES!
//======================================================================================================


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ServiceModel;
using System.Reflection;
using System.Runtime.Serialization;
using ConfigService.ServiceConfigurationContract;
using ConfigService.ServiceConfiguration.DataContract;
using ConfigService.ServiceConfigurationBase;
using ConfigService.ServiceNodeCommunication.DataContract;
using ConfigService.IConfigActions;
using ConfigService.ServiceConfigurationHelper;
using ConfigService.ServiceConfigurationUtility;
using Trade.Utility;
using Trade.BusinessServiceConfigurationSettings;
using Trade.OrderProcessorServiceConfigurationSettings;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorHostConfigurationImplementation;


/* IMPORTANT NOTE:  NORMALLY THIS CLASS WILL SIMPLY INHERIT FROM THE BASE CLASS, AND HAVE NO FURTHER LOGIC. IN OTHER WORDS, NO CODE TO WRITE
 * However, StockTrader Web App is unique in that it allows the BSL and OPS to optionally run in process.  And StockTrader BSL is unique
 * becuase it allows optionally for the OPS to run in-process.  Such modes, for certain configuration operations, require special treatment.
 * The in-process modes are present to enable benchmarking of in-process (monolithic asp.net web app) vs. remote/wcf service call activations.
 * */
namespace Trade.BusinessServiceHostConfigurationImplementation 
{
    /// <summary>
    /// This is the Configuration Service implementation for the Business Service Host. It performs the
    /// operations for service/app configuration.  Note this class now inherits from a base implementation.
    /// You can override any/all methods in the Configuration Service Contract to provide custom implementations, however.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class ConfigurationService : ConfigurationServiceBase
    {

        /// <summary>
        /// Simple constructor.  Make sure to instantiate the settings instance class, which is defined in the base class.
        /// </summary>
        public ConfigurationService()
        {
            settingsInstance = new Trade.BusinessServiceConfigurationSettings.Settings();
        }

        /// <summary>
        /// This is a very key service operation that really makes eveything work fast, across 3 dimensions:
        /// x-horizontal connected services, y-layers deep, virtualized across z-virtual clustered hosts within a given service domain.  It
        /// will be the preceeding call to almost all other Config Service calls, to get the fastest path to a target 
        /// connected Config Service host on the network.
        /// </summary>
        /// <param name="queriedNodeList">This list is used to prevent circular references in a connected tree of nodes from being queried, and creating a race condition.</param>
        /// <param name="targetHostNameIdentifier">The name of the target host we are seeking, as specified in the target's HOST_NAME_IDENTIFIER settings key.</param>
        /// <param name="targetConfigServiceNameIdentifier">The fully qualified name of the Configuration Service implementation class for the host we are seeking, used in
        /// conjunction with targetHostNameIdentifier to uniquely identify a target host.</param>
        /// <param name="csUser">The user requesting the path.</param>
        /// <returns>A network traverse path consisting of a List of TraverseNode types.</returns>
        public override List<TraverseNode> getTraversePath(List<TraverseNode> queriedNodeList, string targetHostNameIdentifier, string targetConfigServiceNameIdentifier, ServiceUsers csUser)
        {
            switch (Trade.BusinessServiceConfigurationSettings.Settings.ORDER_PROCESSING_MODE)
            {
                case StockTraderUtility.OPS_INPROCESS:
                    {
                        ConfigUtility.writeConfigConsoleMessage("Business Service GetTP TARGET: " + targetHostNameIdentifier + ". Looking for: " + StockTraderUtility.QUOTE_SERVICE_AZURE, EventLogEntryType.Information, true, settingsInstance);
                        if (targetHostNameIdentifier.Equals(StockTraderUtility.QUOTE_SERVICE_AZURE))
                        {
                            ServiceConfigHelper configHelper = new ServiceConfigHelper(settingsInstance);
                            TraverseNode myNode = null;
                            List<TraverseNode> thisPath = new List<TraverseNode>();
                            try
                            {
                                if (!(configHelper.setServiceRights(ref csUser, ConfigUtility.CONFIG_CONNECTED_SERVICE_RIGHTS)>=ConfigUtility.CONFIG_CONNECTED_SERVICE_RIGHTS))
                                    return null;
                                ConfigUtility.writeConfigConsoleMessage("\nRequest for traverse path received. Target Service Host: " + targetHostNameIdentifier + " hosting target ConfigService: " + targetConfigServiceNameIdentifier + ".\n", EventLogEntryType.Information, true, settingsInstance);
                                string HOST_NAME_IDENTIFIER = (string)ConfigUtility.reflectGetField(settingsInstance, "HOST_NAME_IDENTIFIER");
                                string CONFIG_SERVICENAME_IDENTIFIER = (string)ConfigUtility.reflectGetField(settingsInstance, "CONFIG_SERVICENAME_IDENTIFIER");
                                int NODE_ACTIVE_ID = (int)ConfigUtility.reflectGetField(settingsInstance, "NODE_ACTIVE_ID");
                                //This logic is used to ensure we construct a valid endpoint based on whether our config service
                                //is using an external load balancer such as Windows Network Load Balancing or hardware load-balancer.  This
                                //would typically be the case for any service exposed to the Internet cloud where clients could be coming from
                                //anywhere and we thus need to virtualize our Internet DNS name--or even intranet DNS name.
                                List<HostedServices> hostedServices = (List<HostedServices>)ConfigUtility.reflectGetField(settingsInstance, "hostedServices");
                                //We may have this service listening on lots of different endpoints.
                                //We need to map the correct Service Endpoint (which implies a binding) to the incoming client request using this WCF property
                                //on the operation context.  However note this is contained in the traversepath (TraverseNode) data structure,
                                //but given new design not currently used within Config Service/ConfigWeb as we dynamically figure out a new valid path on subsequent requests
                                //(given valid paths might change within seconds from original request).
                                //However, returning the correct information here may be important for those extending Config Service, as the
                                //addresses will be valid addresses given incoming request all the way up the stack from the original request.  Those that want could
                                //override this method any choose not to return *any* actual endpoint information for services this service connects to.
                                //Just a choice; independent of security which should always be set on the WCF binding itself (wsHttp or netTcp for ConfigService)
                                //, which, if set  with digital certs, InfoCard, Windows Auth (Kerberos) would be really locked down to identity of caller at the 
                                //transport and/or message level to begin with (meaning we would never be at this point in code if the client was not valid!).
                                string currentRequestEndpoint = OperationContext.Current.EndpointDispatcher.EndpointAddress.ToString();
                                List<HostListenEndPointInstance> myConfigEndpoints = (List<HostListenEndPointInstance>)ConfigUtility.reflectGetField(settingsInstance, "hostConfigListenEndpoints");
                                if (ConfigUtility.onAzure)
                                {
                                    Uri myUri = new Uri(currentRequestEndpoint);
                                    string myHostMachine = myUri.Host;
                                    string path = myUri.AbsolutePath.ToLower();
                                    ConfigUtility.writeConfigConsoleMessage("\n*** currentRequestPath: " + path, EventLogEntryType.Information, true, settingsInstance);
                                    if (myConfigEndpoints == null)
                                        myConfigEndpoints = new List<HostListenEndPointInstance>();
                                    HostListenEndPointInstance theConfigEndpoint = myConfigEndpoints.Find(delegate(HostListenEndPointInstance hExist)
                                    {
                                        if (hExist.RemoteAddress == null)
                                            return false;
                                        Uri uri = new Uri(hExist.RemoteAddress);
                                        return uri.AbsolutePath.ToLower().Equals(path) && uri.Scheme.Equals(myUri.Scheme);
                                    }
                                    );
                                    if (theConfigEndpoint != null)
                                    {
                                        HostedServices thisService = hostedServices.Find(delegate(HostedServices hsExist)
                                        {
                                            return hsExist.HostedServiceID == theConfigEndpoint.HostedServiceID;
                                        }
                                        );
                                        string myName = ServiceConfigHelper.getHostNameForServiceHosting(thisService, settingsInstance);
                                        currentRequestEndpoint = currentRequestEndpoint.Replace(myHostMachine, myName);
                                    }
                                    else
                                        return null;
                                }
                                HostListenEndPointInstance myEp = ServiceConfigHelper.currentRequestFindMatchingListenEndpoint(currentRequestEndpoint, myConfigEndpoints);
                                if (myConfigEndpoints!=null && myConfigEndpoints.Count>0)
                                    ConfigUtility.writeConfigConsoleMessage("\n*** currentRequestEndpoint: " + currentRequestEndpoint + " myConfigEndpoints[0].RemoteAddress: " + myConfigEndpoints[0].RemoteAddress, EventLogEntryType.Information, true, settingsInstance);
                                if (myEp == null)
                                {
                                    ConfigUtility.writeConfigConsoleMessage("myEp is null, returning null path", EventLogEntryType.Information, true, settingsInstance);
                                    return null;
                                }
                                //If we are already in the path, then return null now.  This prevents circular service references within the tree from causing a race
                                //condition.  TODO is to add information about the reference so it shows up in physical maps within ConfigWeb, for now
                                //we will just return null and prevent a race condition.
                                if (queriedNodeList == null)
                                    queriedNodeList = new List<TraverseNode>();

                                TraverseNode me = queriedNodeList.Find(delegate(TraverseNode tn) { return (tn.MyNode.HostNameIdentifier.Equals(HOST_NAME_IDENTIFIER) && tn.MyNode.ConfigServiceImplementationClassName.Equals(CONFIG_SERVICENAME_IDENTIFIER)); });
                                if (me != null)
                                    return null;
                                //Note this does not populate peer nodes such that if using external load balancer, actual physical nodes remain 
                                //hidden behind the virtualized endpoint and are not returned.
                                ConfigUtility.writeConfigConsoleMessage("Getting myConfigEndpoints Node", EventLogEntryType.Information, true, settingsInstance);
                                myNode = new TraverseNode(new ServiceNode(NODE_ACTIVE_ID, myEp.RemoteAddress, ConfigSettings.MESSAGE_ONLINE, myEp.ActiveSince, HOST_NAME_IDENTIFIER, null, CONFIG_SERVICENAME_IDENTIFIER), null);
                                myNode.MyInProcNode = myNode.MyNode;
                                myNode.OSName = OSInfo.Name;
                                queriedNodeList.Add(myNode);
                                thisPath.Add(myNode);
                            }
                            catch (Exception e)
                            {
                                ServiceConfigHelper.logServiceOperationException(csUser, e.ToString(), settingsInstance);
                                throw;
                            }
                            return thisPath;
                        }
                        else
                            return base.getTraversePath(queriedNodeList, targetHostNameIdentifier, targetConfigServiceNameIdentifier, csUser);
                    }
                default:
                    {
                        List<TraverseNode> returnList = base.getTraversePath(queriedNodeList, targetHostNameIdentifier, targetConfigServiceNameIdentifier, csUser);
                        return returnList;
                    }
            }
        }

        /// <summary>
        /// This method returns the composite configuration data for a service--the primary method
        /// used to collect and display the data for a series of connected services making up a composite 
        /// application. 
        /// </summary>
        /// <param name="targetHostNameIdentifier">The name of the target host we are seeking, as specified in the target's HOST_NAME_IDENTIFIER settings key.</param>
        /// <param name="targetConfigServiceName">The fully qualified name of the Configuration Service implementation class for the host we are seeking, used in
        /// conjunction with targetHostNameIdentifier to uniquely identify a target host.</param>
        /// <param name="configurationLevel">The config key level to return.</param>
        /// <param name="probeDeeper">If true, one layer deeper in the connected services tree will be probed as well.</param>
        /// <param name="traversePath">The network traverse path to the target.</param>
        /// <param name="csUser">User to authenticate.</param>
        /// <returns>Linked List of ServiceConfigurationData.</returns>
        public override List<ServiceConfigurationData> getServiceConfiguration(string targetHostNameIdentifier, string targetConfigServiceName, int configurationLevel, bool probeDeeper, List<TraverseNode> traversePath, ServiceUsers csUser)
        {
            ServiceConfigHelper configHelper = new ServiceConfigHelper(settingsInstance);
            object opsSettings = new Trade.OrderProcessorServiceConfigurationSettings.Settings();
            List<ServiceConfigurationData> returnCompositeConfigurationData = null;
            try
            {
                if (!(configHelper.setServiceRights(ref csUser, ConfigUtility.CONFIG_CONNECTED_SERVICE_RIGHTS) >= ConfigUtility.CONFIG_CONNECTED_SERVICE_RIGHTS))
                {
                    returnCompositeConfigurationData = new List<ServiceConfigurationData>();
                    return returnCompositeConfigurationData;
                }
                if (traversePath == null || traversePath.Count <= 1 || traversePath[0] == null)
                {
                    //Step 1:  Get list of all currently configured connected services.  We need to adjust this list based on
                    //the OrderMode setting, since we do not want to query for and return connected data for the Order Processor Service
                    //if we are not running Order Processing remotely. 
                    List<ConnectedServices> retrieveConnectedServiceList = new List<ConnectedServices>();
                    if (Trade.BusinessServiceConfigurationSettings.Settings.connectedServices != null && Trade.BusinessServiceConfigurationSettings.Settings.connectedServices.Count > 0)
                        retrieveConnectedServiceList.AddRange(Trade.BusinessServiceConfigurationSettings.Settings.connectedServices);

                    //Step 2: Here we will remove the Order Processor Service if we are not running in a remote mode. 
                    switch (Trade.BusinessServiceConfigurationSettings.Settings.ORDER_PROCESSING_MODE)
                    {
                        case StockTraderUtility.OPS_INPROCESS:
                            {
                                //In this case, we want no Business Service connections--so remove by contract name.
                                retrieveConnectedServiceList.RemoveAll(delegate(ConnectedServices cs) { return (cs.ServiceContract.Equals(Trade.BusinessServiceConfigurationSettings.Settings.OPS_CLIENT_CONNECTED_SERVICE_CONTRACT)); });
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                    returnCompositeConfigurationData = configHelper.getMyServiceConfiguration(targetHostNameIdentifier, targetConfigServiceName, configurationLevel, csUser, retrieveConnectedServiceList, probeDeeper);
                    //Now another special case for the Business Services app: it can run OPS Services in-process.  In this case,
                    //we will add data for OPS Services by invoking the config service in-process, and pointing to the OPS self-host repository.
                    if (BusinessServiceConfigurationSettings.Settings.ORDER_PROCESSING_MODE == StockTraderUtility.OPS_INPROCESS)
                    {
                        List<ServiceConfigurationData> opsConfigData = null;
                        Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService opsServiceConfig = new Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService();
                        if (ConfigUtility.onAzure)
                            opsConfigData = opsServiceConfig.getServiceConfiguration(StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE, StockTraderUtility.ORDER_PROCESSOR_SERVICE_CONFIG, configurationLevel, false, traversePath, csUser);
                        if (opsConfigData != null)
                        {
                            for (int i = 0; i < opsConfigData.Count; i++)
                            {
                                opsConfigData[i].InProcessHost = Trade.BusinessServiceConfigurationSettings.Settings.HOST_NAME_IDENTIFIER;
                            }
                            returnCompositeConfigurationData.AddRange(opsConfigData);
                        }
                    }
                }
                else
                {
                    if (traversePath != null && traversePath.Count > 1)
                    {
                        ConfigUtility.writeConfigConsoleMessage("-----> Forwarding Request to Remote Connected Service: " + traversePath[traversePath.Count - 1].MyNode.Address + "\n", EventLogEntryType.Information, true, settingsInstance);
                        RemoteNotifications remoteNotify = new RemoteNotifications(settingsInstance);
                        returnCompositeConfigurationData = remoteNotify.getRemoteServiceConfiguration(targetHostNameIdentifier, targetConfigServiceName, configurationLevel, probeDeeper, traversePath, csUser, false, null);
                    }
                }
            }
            catch (Exception e)
            {
                ServiceConfigHelper.logServiceOperationException(csUser, e.ToString(), settingsInstance);
                throw;
            }
            return returnCompositeConfigurationData;
        }

        /// <summary>
        /// This method retrieves a specific configuration key from a configuration database for a service host.
        /// </summary>
        /// <param name="targetHostNameIdentifier">The name of the target host we are seeking, as specified in the target's HOST_NAME_IDENTIFIER settings key.</param>
        /// <param name="targetConfigServiceName">The fully qualified name of the Configuration Service implementation class for the host we are seeking, used in
        /// conjunction with targetHostNameIdentifier to uniquely identify a target host.</param>
        /// <param name="settingsClassFieldName">The field name which is the primary key in the ConfigurationKeys table.</param>
        /// <param name="traversePath">Network path to target host.</param>
        /// <param name="csUser">User to authenticate.</param>
        /// <returns>A configuration key in form of data contract class ConfigurationKeyValues.</returns>
        public override ConfigurationKeyValues getServiceConfigurationKey(string targetHostNameIdentifier, string targetConfigServiceNameIdentifier, string settingsClassFieldName, List<TraverseNode> traversePath, ServiceUsers csUser)
        {
            ServiceConfigHelper configHelper = new ServiceConfigHelper(settingsInstance);
            object opsSettings = new Trade.OrderProcessorServiceConfigurationSettings.Settings();
            ConfigurationKeyValues returnkey = null;
            try
            {
                if (!(configHelper.setServiceRights(ref csUser, ConfigUtility.CONFIG_ADMIN_RIGHTS)>=ConfigUtility.CONFIG_ADMIN_RIGHTS))
                    return null;
                ConfigUtility.writeConfigConsoleMessage("\nRequest for configuration key received.\n", EventLogEntryType.Information, true, settingsInstance);
                if (traversePath == null || traversePath.Count <= 1 || traversePath[0] == null)
                {
                    bool SHARESERVICECONFIGURATIONDATA = (bool)ConfigUtility.reflectGetField(settingsInstance, "SHARESERVICECONFIGURATIONDATA");
                    if (!SHARESERVICECONFIGURATIONDATA)
                    {
                        List<ServiceUsers> thisServiceUsers = (List<ServiceUsers>)ConfigUtility.reflectGetField(settingsInstance, "thisServiceUsers");
                        ServiceUsers userLocalAdmin = thisServiceUsers.Find(delegate(ServiceUsers userExist) { return csUser.UserId.ToLower().Equals("localadmin") && userExist.Password.Equals(csUser.Password) && csUser.Rights == ConfigUtility.CONFIG_ADMIN_RIGHTS; });
                        if (userLocalAdmin == null)
                            return null;
                    }
                    if (BusinessServiceConfigurationSettings.Settings.ORDER_PROCESSING_MODE == StockTraderUtility.OPS_INPROCESS)
                    {
                        switch (targetHostNameIdentifier)
                        {
                            case StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE:
                                {
                                    returnkey = callOPSConfigKeyLocal(targetHostNameIdentifier, targetConfigServiceNameIdentifier, settingsClassFieldName, traversePath, csUser);
                                    break;
                                }

                            default:
                                {
                                    returnkey = configHelper.getServiceConfigurationKey(settingsClassFieldName);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        returnkey = configHelper.getServiceConfigurationKey(settingsClassFieldName);
                    }
                }
                else
                {
                    if (traversePath != null && traversePath.Count > 1)
                    {
                        ConfigUtility.writeConfigConsoleMessage("-----> Forwarding Request to Remote Connected Service: " + traversePath[traversePath.Count - 1].MyNode.Address + "\n", EventLogEntryType.Information, true, settingsInstance);
                        RemoteNotifications remoteNotify = new RemoteNotifications(settingsInstance);
                        returnkey = remoteNotify.getRemoteServiceKey(targetHostNameIdentifier, targetConfigServiceNameIdentifier, settingsClassFieldName, traversePath, csUser);
                    }
                }
            }
            catch (Exception e)
            {
                ServiceConfigHelper.logServiceOperationException(csUser, e.ToString(), settingsInstance);
                throw;
            }
            return returnkey;
        }

        /// <summary>
        /// This method performs the logic required when a specific configuration settings key is changed.  This includes 
        /// potentially updating configuration database, notifying nodes, and performing configuration actions.
        /// </summary>
        /// <param name="targetHostNameIdentifier">The name of the target host we are seeking, as specified in the target's HOST_NAME_IDENTIFIER settings key.</param>
        /// <param name="targetConfigServiceName">The fully qualified name of the Configuration Service implementation class for the host we are seeking, used in
        /// conjunction with targetHostNameIdentifier to uniquely identify a target host.</param>
        /// <param name="oldKey">The key prior to update.</param>
        /// <param name="newKey">The updated key.</param>
        /// <param name="notifyNodes">If true, key update will be flowed to peer nodes.</param>
        /// <param name="action">Action to perform.</param>
        /// <param name="traversePath">Network path to the target host.</param>
        /// <param name="csUser">User to authenticate.</param>
        /// <returns>Int representing success code.</returns>
        public override int receiveConfigurationKey(string targetHostNameIdentifier, string targetConfigServiceNameIdentifier, ConfigurationKeyValues oldKey, ConfigurationKeyValues newKey, bool notifyNodes, string action, List<TraverseNode> traversePath, ServiceUsers csUser)
        {
            ServiceConfigHelper configHelper = new ServiceConfigHelper(settingsInstance);
            bool success = true;
            object theSettingsInstance = new Trade.BusinessServiceConfigurationSettings.Settings();
            IConfigurationActions configActions;
            int returnCode = ConfigUtility.CLUSTER_UPDATE_FULL_SUCCESS;
            try
            {
                if (!(configHelper.setServiceRights(ref csUser, ConfigUtility.CONFIG_ADMIN_RIGHTS)>=ConfigUtility.CONFIG_ADMIN_RIGHTS))
                    return ConfigUtility.CLUSTER_UPDATE_FAIL_AUTHENTICATION;
                configActions = (IConfigurationActions)ConfigUtility.reflectGetField(settingsInstance, "thisServiceConfigActions");
                if ((action != ConfigUtility.ADD_KEY && action != ConfigUtility.REMOVE_KEY) && (traversePath == null || traversePath.Count <= 1 || traversePath[0] == null))
                {
                    if (targetHostNameIdentifier != StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE)
                    {
                        if (!configActions.validateConfigurationKey(settingsInstance, newKey))
                            return ConfigUtility.CLUSTER_UPDATE_FAIL_VALIDATION;
                    }
                }
                ConfigUtility.writeConfigConsoleMessage("\nReceived Request for Configuration Key Change, Action: " + action + ". Keyname: " + oldKey.ConfigurationKeyDisplayName + "\n", EventLogEntryType.Information, true, settingsInstance);
                ConfigUtility.writeConfigConsoleMessage("Value: " + newKey.ConfigurationKeyValue + "\n\n", EventLogEntryType.Information, true, settingsInstance);
                if (traversePath == null || traversePath.Count <= 1 || traversePath[0] == null)
                {
                    bool SHARESERVICECONFIGURATIONDATA = (bool)ConfigUtility.reflectGetField(settingsInstance, "SHARESERVICECONFIGURATIONDATA");
                    if (!SHARESERVICECONFIGURATIONDATA)
                    {
                        List<ServiceUsers> thisServiceUsers = (List<ServiceUsers>)ConfigUtility.reflectGetField(settingsInstance, "thisServiceUsers");
                        ServiceUsers userLocalAdmin = thisServiceUsers.Find(delegate(ServiceUsers userExist) { return csUser.UserId.ToLower().Equals("localadmin") && userExist.Password.Equals(csUser.Password) && csUser.Rights == ConfigUtility.CONFIG_ADMIN_RIGHTS; });
                        if (userLocalAdmin == null)
                            return ConfigUtility.CLUSTER_UPDATE_FULL_SUCCESS;
                    }
                    switch (targetHostNameIdentifier)
                    {
                        case StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE:
                            {
                                theSettingsInstance = new Trade.OrderProcessorServiceConfigurationSettings.Settings();
                                Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService configService = new Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService();
                                int opsRetCode = configService.receiveConfigurationKey(targetHostNameIdentifier, targetConfigServiceNameIdentifier, oldKey, newKey, notifyNodes, action, traversePath, csUser);
                                if (opsRetCode == ConfigUtility.CLUSTER_UPDATE_FULL_SUCCESS)
                                    success = true;
                                break;
                            }

                        
                        default:
                            {
                                if (notifyNodes)
                                    success = configHelper.sendConfigurationKeyUpdateLocal(oldKey, newKey, action, csUser);
                                break;
                            }
                    }
                    if (success)
                    {
                        if (targetHostNameIdentifier != StockTraderUtility.ORDER_PROCESSOR_SERVICE_AZURE)
                            success = configActions.checkChangedSetting(theSettingsInstance, newKey, notifyNodes, csUser);
                        if (notifyNodes && success)
                        {
                            RemoteNotifications remoteNotify = new RemoteNotifications(settingsInstance);
                            return remoteNotify.sendKeyUpdateToClusterMembers(targetHostNameIdentifier, targetConfigServiceNameIdentifier, oldKey, newKey, action);
                        }
                    }
                    else
                        returnCode = ConfigUtility.CLUSTER_UPDATE_FAIL_PERSISTED;
                }
                else
                {
                    if (traversePath != null && traversePath.Count > 1)
                    {
                        RemoteNotifications remoteKeyUpdate = new RemoteNotifications(settingsInstance);
                        ConfigUtility.writeConfigConsoleMessage("     Forwarding Key to Remote Connected Service: " + traversePath[traversePath.Count - 1].MyNode.Address + "\n", EventLogEntryType.Information, true, settingsInstance);
                        return (remoteKeyUpdate.sendConfigurationKeyUpdateRemote(targetHostNameIdentifier, targetConfigServiceNameIdentifier, oldKey, newKey, notifyNodes, action, traversePath, csUser));
                    }
                }
            }
            catch (Exception e)
            {
                ServiceConfigHelper.logServiceOperationException(csUser, e.ToString(), settingsInstance);
                throw;
            }
            return (returnCode);
        }


        /// <summary>
        /// Will return the Primary and Generic Service Connection Points from the ConnectedServiceInstances table in the configuration database. 
        /// </summary>
        /// <param name="targetHostNameIdentifier">The name of the target host we are seeking, as specified in the target's HOST_NAME_IDENTIFIER settings key.</param>
        /// <param name="targetConfigServiceName">The fully qualified name of the Configuration Service implementation class for the host we are seeking, used in
        /// conjunction with targetHostNameIdentifier to uniquely identify a target host.</param>
        /// <param name="connectionPointTypes">The type of connection points to retrieve.</param>
        /// <param name="checkStatus">If true, a satus flag will be set indicating online status of the endpoint.</param>
        /// <param name="traversePath">Network path to the target host.</param>
        /// <param name="csUser">User to authenticate.</param>
        /// <returns>ConnectionPoints instance which is a linked list type.</returns>
        public override ConnectionPoints getConnectionPoints(string targetHostNameIdentifier, string targetConfigServiceName, List<int> connectionPointTypes, bool checkStatus, List<TraverseNode> traversePath, ServiceUsers csUser)
        {
            ServiceConfigHelper configHelper = new ServiceConfigHelper(settingsInstance);
            ConnectionPoints returnConnectionPoints = null;
            try
            {
                if (!(configHelper.setServiceRights(ref csUser, ConfigUtility.CONFIG_DEMO_ADMIN_RIGHTS) >= ConfigUtility.CONFIG_DEMO_ADMIN_RIGHTS))
                    return null;
                ConfigUtility.writeConfigConsoleMessage("\nRequest for Connection Points received.\n", EventLogEntryType.Information, true, settingsInstance);
                if (traversePath == null || traversePath.Count <= 1 || traversePath[0] == null)
                {
                    bool SHARESERVICECONFIGURATIONDATA = (bool)ConfigUtility.reflectGetField(settingsInstance, "SHARESERVICECONFIGURATIONDATA");
                    if (!SHARESERVICECONFIGURATIONDATA)
                    {
                        List<ServiceUsers> thisServiceUsers = (List<ServiceUsers>)ConfigUtility.reflectGetField(settingsInstance, "thisServiceUsers");
                        ServiceUsers userLocalAdmin = thisServiceUsers.Find(delegate(ServiceUsers userExist) { return csUser.UserId.ToLower().Equals("localadmin") && userExist.Password.Equals(csUser.Password) && csUser.Rights == ConfigUtility.CONFIG_ADMIN_RIGHTS; });
                        if (userLocalAdmin == null)
                            return null;
                    }
                    returnConnectionPoints = configHelper.getMyConnectionPoints(connectionPointTypes);
                    string HOST_NAME_IDENTIFIER = (string)ConfigUtility.reflectGetField(settingsInstance, "HOST_NAME_IDENTIFIER");
                    string CONFIG_SERVICENAME_IDENTIFIER = (string)ConfigUtility.reflectGetField(settingsInstance, "CONFIG_SERVICENAME_IDENTIFIER");
                    bool GETCONNECTEDSERVICES = (bool)ConfigUtility.reflectGetField(settingsInstance, "CONFIGSHOWCONNECTEDSERVICES");
                    returnConnectionPoints.HostNameIdentifier = HOST_NAME_IDENTIFIER;
                    List<HostListenEndPointInstance> checkOnlineList = new List<HostListenEndPointInstance>();
                    for (int i = 0; i < returnConnectionPoints.MyConnectionPoints.Count; i++)
                    {
                        switch (Trade.BusinessServiceConfigurationSettings.Settings.ORDER_PROCESSING_MODE)
                        {
                            case StockTraderUtility.OPS_INPROCESS:
                                {
                                    break;
                                }

                            case StockTraderUtility.OPS_AZURE_HTTP_TMSEC_CLIENTCERT:
                                {
                                    if (returnConnectionPoints.MyConnectionPoints[i].ServiceFriendlyName.Equals(StockTraderUtility.OPS_AZURE_HTTP_TMSEC_CLIENTCERT))
                                        returnConnectionPoints.MyConnectionPoints[i].InUse = true;
                                    break;
                                }

                            case StockTraderUtility.OPS_AZURE_HTTP_TMSEC_USERNAME:
                                {
                                    if (returnConnectionPoints.MyConnectionPoints[i].ServiceFriendlyName.Equals(StockTraderUtility.OPS_AZURE_HTTP_TMSEC_USERNAME))
                                        returnConnectionPoints.MyConnectionPoints[i].InUse = true;
                                    break;
                                }

                            case StockTraderUtility.OPS_AZURE_TCP_TMSEC_CLIENTCERT:
                                {
                                    if (returnConnectionPoints.MyConnectionPoints[i].ServiceFriendlyName.Equals(StockTraderUtility.OPS_AZURE_TCP_TMSEC_CLIENTCERT))
                                        returnConnectionPoints.MyConnectionPoints[i].InUse = true;
                                    break;
                                }

                            case StockTraderUtility.OPS_AZURE_TCP_TMSEC_USERNAME:
                                {
                                    if (returnConnectionPoints.MyConnectionPoints[i].ServiceFriendlyName.Equals(StockTraderUtility.OPS_AZURE_TCP_TMSEC_USERNAME))
                                        returnConnectionPoints.MyConnectionPoints[i].InUse = true;
                                    break;
                                }
                        }
                        if (checkStatus)
                        {
                            checkOnlineList.Add(returnConnectionPoints.MyConnectionPoints[i]);
                        }
                    }
                    if (checkOnlineList.Count > 0)
                    {
                        RemoteNotifications remoteNotify = new RemoteNotifications(settingsInstance);
                        //This call will set online offline status for each; all are checked concurrently on background threads;
                        //Master thread joined to ensure all are done before returning.
                        remoteNotify.checkConnectionPointOnlineStatus(checkOnlineList, false);
                    }
                }
                else
                {
                    if (traversePath != null && traversePath.Count > 1)
                    {
                        ConfigUtility.writeConfigConsoleMessage("-----> Forwarding Request to Remote Connected Service: " + traversePath[traversePath.Count - 1].MyNode.Address + "\n", EventLogEntryType.Information, true, settingsInstance);
                        RemoteNotifications remoteNotify = new RemoteNotifications(settingsInstance);
                        returnConnectionPoints = remoteNotify.getRemoteConnectionPoints(targetHostNameIdentifier, targetConfigServiceName, connectionPointTypes, checkStatus, traversePath, csUser);
                    }
                }
            }
            catch (Exception e)
            {
                ServiceConfigHelper.logServiceOperationException(csUser, e.ToString(), settingsInstance);
                throw;
            }
            return returnConnectionPoints;
        }

        /// <summary>
        /// Not part of service contract; special for BSL to return a single requested specific configurationkey 
        /// from Order Processor Service when running OPS in-process.
        /// </summary>
        /// <param name="targetHostNameIdentifier"></param>
        /// <param name="targetConfigServiceNameIdentifier"></param>
        /// <param name="settingsClassFieldName"></param>
        /// <param name="traversePath"></param>
        /// <param name="csUser"></param>
        /// <returns></returns>
        private ConfigurationKeyValues callOPSConfigKeyLocal(string targetHostNameIdentifier, string targetConfigServiceNameIdentifier, string settingsClassFieldName, List<TraverseNode> traversePath, ServiceUsers csUser)
        {
            Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService OPSConfig = new Trade.OrderProcessorHostConfigurationImplementation.ConfigurationService();
            return OPSConfig.getServiceConfigurationKey(targetHostNameIdentifier,targetConfigServiceNameIdentifier,settingsClassFieldName,traversePath,csUser);
        }
    }
}

