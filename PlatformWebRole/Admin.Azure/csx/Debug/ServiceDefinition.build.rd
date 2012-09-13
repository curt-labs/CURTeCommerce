<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Admin.Azure" generation="1" functional="0" release="0" Id="45247f22-b8a0-4c0f-ba16-30b8167efd1d" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Admin.AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Admin:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Admin.Azure/Admin.AzureGroup/LB:Admin:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Admin:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Admin.Azure/Admin.AzureGroup/MapAdmin:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="AdminInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Admin.Azure/Admin.AzureGroup/MapAdminInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Admin:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Admin.Azure/Admin.AzureGroup/Admin/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapAdmin:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Admin.Azure/Admin.AzureGroup/Admin/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapAdminInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Admin.Azure/Admin.AzureGroup/AdminInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Admin" generation="1" functional="0" release="0" software="C:\Users\jjaniuk\Projects\eCommercePlatform\PlatformWebRole\Admin.Azure\csx\Debug\roles\Admin" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Admin&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Admin&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Admin.Azure/Admin.AzureGroup/AdminInstances" />
            <sCSPolicyFaultDomainMoniker name="/Admin.Azure/Admin.AzureGroup/AdminFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="AdminFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="AdminInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="bacf4e82-a766-4b2a-b6f3-511abb400c54" ref="Microsoft.RedDog.Contract\ServiceContract\Admin.AzureContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="1c4fa2b8-fd72-40c8-b2af-f329a5b2a0c6" ref="Microsoft.RedDog.Contract\Interface\Admin:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Admin.Azure/Admin.AzureGroup/Admin:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>