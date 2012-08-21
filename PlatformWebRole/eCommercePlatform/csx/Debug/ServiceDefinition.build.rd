<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="eCommercePlatform" generation="1" functional="0" release="0" Id="e776dac4-7cd7-4866-a1a1-e6ed1feb37d6" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="eCommercePlatformGroup" generation="1" functional="0" release="0">
      <componentports>
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
        <inPort name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/LB:PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapCertificate|PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="Certificate|PlatformWebRole:NewSSLCert" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapCertificate|PlatformWebRole:NewSSLCert" />
          </maps>
        </aCS>
        <aCS name="Certificate|PlatformWebRole:NoBarSSLCert" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapCertificate|PlatformWebRole:NoBarSSLCert" />
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
        <aCS name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/eCommercePlatform/eCommercePlatformGroup/MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
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
        <lBChannel name="LB:PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:TaskScheduler:Endpoint1">
          <toPorts>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/TaskScheduler/Endpoint1" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapCertificate|PlatformWebRole:NewSSLCert" kind="Identity">
          <certificate>
            <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/NewSSLCert" />
          </certificate>
        </map>
        <map name="MapCertificate|PlatformWebRole:NoBarSSLCert" kind="Identity">
          <certificate>
            <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/NoBarSSLCert" />
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
        <map name="MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapPlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
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
          <role name="FTPServerRole" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\FTPServerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="FTP" protocol="tcp" portRanges="21" />
              <outPort name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
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
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FTPServerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
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
          <role name="PlatformWebRole" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\PlatformWebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="3584" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="Https" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/NoBarSSLCert" />
                </certificate>
              </inPort>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
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
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="Mode" defaultValue="" />
              <aCS name="ProviderName" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="UseAsyncMethods" defaultValue="" />
              <aCS name="UseHttps" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;PlatformWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
              <storedCertificate name="Stored1NewSSLCert" certificateStore="CA" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/NewSSLCert" />
                </certificate>
              </storedCertificate>
              <storedCertificate name="Stored2NoBarSSLCert" certificateStore="CA" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole/NoBarSSLCert" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
              <certificate name="NewSSLCert" />
              <certificate name="NoBarSSLCert" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRoleInstances" />
            <sCSPolicyFaultDomainMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="TaskScheduler" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\eCommercePlatform\csx\Debug\roles\TaskScheduler" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="tcp" />
              <outPort name="PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eCommercePlatform/eCommercePlatformGroup/SW:PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
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
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;TaskScheduler&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PlatformWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Https&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;TaskScheduler&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
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
        <sCSPolicyFaultDomain name="FTPServerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="PlatformWebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="TaskSchedulerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FTPServerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="PlatformWebRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="TaskSchedulerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="e40a11ae-e101-406a-8f2c-a3712bd8a183" ref="Microsoft.RedDog.Contract\ServiceContract\eCommercePlatformContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="f81ccda4-838a-4a9a-aea6-63595934c935" ref="Microsoft.RedDog.Contract\Interface\FTPServerRole:FTP@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/FTPServerRole:FTP" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="f1796867-e757-485f-b41f-689b0dd0b321" ref="Microsoft.RedDog.Contract\Interface\PlatformWebRole:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="5a0c14b8-ddfd-415c-ae9f-31a0d1491cc4" ref="Microsoft.RedDog.Contract\Interface\PlatformWebRole:Https@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole:Https" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="142ae6ad-c386-4341-9c72-4994a022c6b9" ref="Microsoft.RedDog.Contract\Interface\PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/eCommercePlatform/eCommercePlatformGroup/PlatformWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>