<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="AzureStockTrader" generation="1" functional="0" release="0" Id="4e55685a-b2d2-495a-a1ba-8dde33dc4f62" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzureStockTraderGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Trade_WebRole:HttpsEndpoint" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/AzureStockTrader/AzureStockTraderGroup/LB:Trade_WebRole:HttpsEndpoint" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|Trade_WebRole:StockTraderBSLClient.Com" defaultValue="">
          <maps>
            <mapMoniker name="/AzureStockTrader/AzureStockTraderGroup/MapCertificate|Trade_WebRole:StockTraderBSLClient.Com" />
          </maps>
        </aCS>
        <aCS name="Certificate|Trade_WebRole:StockTraderWebAppSSL.Com" defaultValue="">
          <maps>
            <mapMoniker name="/AzureStockTrader/AzureStockTraderGroup/MapCertificate|Trade_WebRole:StockTraderWebAppSSL.Com" />
          </maps>
        </aCS>
        <aCS name="Trade_WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/AzureStockTrader/AzureStockTraderGroup/MapTrade_WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="Trade_WebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/AzureStockTrader/AzureStockTraderGroup/MapTrade_WebRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Trade_WebRole:HttpsEndpoint">
          <toPorts>
            <inPortMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/HttpsEndpoint" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:Trade_WebRole:NodeServiceEndpointHttp">
          <toPorts>
            <inPortMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/NodeServiceEndpointHttp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|Trade_WebRole:StockTraderBSLClient.Com" kind="Identity">
          <certificate>
            <certificateMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/StockTraderBSLClient.Com" />
          </certificate>
        </map>
        <map name="MapCertificate|Trade_WebRole:StockTraderWebAppSSL.Com" kind="Identity">
          <certificate>
            <certificateMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/StockTraderWebAppSSL.Com" />
          </certificate>
        </map>
        <map name="MapTrade_WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapTrade_WebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Trade_WebRole" generation="1" functional="0" release="0" software="E:\Work\General Research\Azure Stock Trader 6.0\StockTrader\StockTraderWebApplication\AzureStockTrader\csx\Debug\roles\Trade_WebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="HttpsEndpoint" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/StockTraderWebAppSSL.Com" />
                </certificate>
              </inPort>
              <inPort name="NodeServiceEndpointHttp" protocol="http" />
              <outPort name="Trade_WebRole:NodeServiceEndpointHttp" protocol="http">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/AzureStockTrader/AzureStockTraderGroup/SW:Trade_WebRole:NodeServiceEndpointHttp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Trade_WebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Trade_WebRole&quot;&gt;&lt;e name=&quot;HttpsEndpoint&quot; /&gt;&lt;e name=&quot;NodeServiceEndpointHttp&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0StockTraderBSLClient.Com" certificateStore="TrustedPeople" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/StockTraderBSLClient.Com" />
                </certificate>
              </storedCertificate>
              <storedCertificate name="Stored1StockTraderWebAppSSL.Com" certificateStore="TrustedPeople" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole/StockTraderWebAppSSL.Com" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="StockTraderBSLClient.Com" />
              <certificate name="StockTraderWebAppSSL.Com" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="Trade_WebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="Trade_WebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="Trade_WebRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="6ca7f258-bad3-46c6-9e86-8fa7106ca085" ref="Microsoft.RedDog.Contract\ServiceContract\AzureStockTraderContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="75253306-74ef-49dc-9d39-35b57029667e" ref="Microsoft.RedDog.Contract\Interface\Trade_WebRole:HttpsEndpoint@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzureStockTrader/AzureStockTraderGroup/Trade_WebRole:HttpsEndpoint" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>