<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="eCommercePlatform" generation="1" functional="0" release="0" Id="292ffca6-d499-4b00-8e52-aaf60f226c60" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="eCommercePlatformGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Admin:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/LB:Admin:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="FTPServerRole:FTP" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/LB:FTPServerRole:FTP" />
          </inToChannel>
        </inPort>
        <inPort name="PlatformWebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/LB:PlatformWebRole:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="PlatformWebRole:Https" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/LB:PlatformWebRole:Https" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Admin:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapAdmin:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="AdminInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapAdminInstances" />
          </maps>
        </aCS>
        <aCS name="Certificate|PlatformWebRole:CurtSSL" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapCertificate|PlatformWebRole:CurtSSL" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:AccountKey" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:AccountKey" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:AccountName" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:AccountName" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:BaseUri" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:BaseUri" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:Mode" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:Mode" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:ProviderName" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:ProviderName" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:UseAsyncMethods" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:UseAsyncMethods" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:UseHttps" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRole:UseHttps" />
          </maps>
        </aCS>
        <aCS name="FTPServerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapFTPServerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:AccountKey" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:AccountKey" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:AccountName" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:AccountName" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:BaseUri" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:BaseUri" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:Mode" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:Mode" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:ProviderName" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:ProviderName" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:UseAsyncMethods" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:UseAsyncMethods" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:UseHttps" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:UseHttps" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRoleInstances" />
          </maps>
        </aCS>
        <aCS name="TaskScheduler:AccountKey" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapTaskScheduler:AccountKey" />
          </maps>
        </aCS>
        <aCS name="TaskScheduler:AccountName" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapTaskScheduler:AccountName" />
          </maps>
        </aCS>
        <aCS name="TaskScheduler:ProviderName" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapTaskScheduler:ProviderName" />
          </maps>
        </aCS>
        <aCS name="TaskScheduler:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapTaskScheduler:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="TaskSchedulerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapTaskSchedulerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Admin:Endpoint1">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/Admin/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPServerRole:FTP">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/FTP" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:PlatformWebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:PlatformWebRole:Https">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Https" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:TaskScheduler:Endpoint1">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskScheduler/Endpoint1" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapAdmin:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/Admin/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapAdminInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/AdminInstances" />
          </setting>
        </map>
        <map name="MapCertificate|PlatformWebRole:CurtSSL" kind="Identity">
          <certificate>
            <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/CurtSSL" />
          </certificate>
        </map>
        <map name="MapFTPServerRole:AccountKey" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/AccountKey" />
          </setting>
        </map>
        <map name="MapFTPServerRole:AccountName" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/AccountName" />
          </setting>
        </map>
        <map name="MapFTPServerRole:BaseUri" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/BaseUri" />
          </setting>
        </map>
        <map name="MapFTPServerRole:Mode" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/Mode" />
          </setting>
        </map>
        <map name="MapFTPServerRole:ProviderName" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/ProviderName" />
          </setting>
        </map>
        <map name="MapFTPServerRole:UseAsyncMethods" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/UseAsyncMethods" />
          </setting>
        </map>
        <map name="MapFTPServerRole:UseHttps" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole/UseHttps" />
          </setting>
        </map>
        <map name="MapFTPServerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRoleInstances" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:AccountKey" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/AccountKey" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:AccountName" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/AccountName" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:BaseUri" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/BaseUri" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:Mode" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Mode" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:ProviderName" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/ProviderName" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:UseAsyncMethods" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/UseAsyncMethods" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:UseHttps" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/UseHttps" />
          </setting>
        </map>
        <map name="MapPlatformWebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRoleInstances" />
          </setting>
        </map>
        <map name="MapTaskScheduler:AccountKey" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskScheduler/AccountKey" />
          </setting>
        </map>
        <map name="MapTaskScheduler:AccountName" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskScheduler/AccountName" />
          </setting>
        </map>
        <map name="MapTaskScheduler:ProviderName" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskScheduler/ProviderName" />
          </setting>
        </map>
        <map name="MapTaskScheduler:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskScheduler/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapTaskSchedulerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskSchedulerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Admin" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\Admin" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="8080" />
              <outPort name="TaskScheduler:Endpoint1" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:TaskScheduler:Endpoint1" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Admin&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Admin&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/AdminInstances" />
            <sCSPolicyFaultDomainMoniker name="/eCommercePlatform/eCommercePlatformGroup/AdminFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="FTPServerRole" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\FTPServerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="768" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="FTP" protocol="tcp" portRanges="21" />
              <outPort name="TaskScheduler:Endpoint1" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:TaskScheduler:Endpoint1" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="AccountKey" defaultValue="" />
              <aCS name="AccountName" defaultValue="" />
              <aCS name="BaseUri" defaultValue="" />
              <aCS name="Mode" defaultValue="" />
              <aCS name="ProviderName" defaultValue="" />
              <aCS name="UseAsyncMethods" defaultValue="" />
              <aCS name="UseHttps" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FTPServerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Admin&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRoleInstances" />
            <sCSPolicyFaultDomainMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="PlatformWebRole" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\PlatformWebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="Https" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/CurtSSL" />
                </certificate>
              </inPort>
              <outPort name="TaskScheduler:Endpoint1" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:TaskScheduler:Endpoint1" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="AccountKey" defaultValue="" />
              <aCS name="AccountName" defaultValue="" />
              <aCS name="BaseUri" defaultValue="" />
              <aCS name="Mode" defaultValue="" />
              <aCS name="ProviderName" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="UseAsyncMethods" defaultValue="" />
              <aCS name="UseHttps" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;PlatformWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Admin&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0CurtSSL" certificateStore="CA" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/CurtSSL" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="CurtSSL" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRoleInstances" />
            <sCSPolicyFaultDomainMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="TaskScheduler" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\TaskScheduler" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="768" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="tcp" />
              <outPort name="TaskScheduler:Endpoint1" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:TaskScheduler:Endpoint1" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="AccountKey" defaultValue="" />
              <aCS name="AccountName" defaultValue="" />
              <aCS name="ProviderName" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;TaskScheduler&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Admin&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskSchedulerInstances" />
            <sCSPolicyFaultDomainMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskSchedulerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="AdminFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="FTPServerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="PlatformWebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="TaskSchedulerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="AdminInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="FTPServerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="PlatformWebRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="TaskSchedulerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="233e2e2c-498d-4cf1-a759-04e68eec0845" ref="Microsoft.RedDog.Contract\ServiceContract\eCommercePlatformContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="020a12ea-88c2-401f-a051-59ebf8161235" ref="Microsoft.RedDog.Contract\Interface\Admin:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/Admin:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="8082dbb7-cef2-4b61-9eea-85db5e7939c8" ref="Microsoft.RedDog.Contract\Interface\FTPServerRole:FTP@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole:FTP" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="83c944b0-b7f2-4440-997a-e739f975757a" ref="Microsoft.RedDog.Contract\Interface\PlatformWebRole:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="efa56d47-bfa2-4424-a763-f22ecb97ccaf" ref="Microsoft.RedDog.Contract\Interface\PlatformWebRole:Https@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole:Https" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>