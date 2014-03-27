<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="1e272c42-8c6a-4f16-9329-58de94f55a53" namespace="CarbonKnown.FileWatcherService" xmlSchemaNamespace="urn:CarbonKnown.FileWatcherService" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="FileWatcherConfigSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="fileWatcherConfigSection">
      <elementProperties>
        <elementProperty name="FileHandlers" isRequired="true" isKey="false" isDefaultCollection="true" xmlName="fileHandlers" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/HandlerCollection" />
          </type>
        </elementProperty>
        <elementProperty name="HandlerGroups" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="handlerGroups" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/HandlerGroupCollection" />
          </type>
        </elementProperty>
        <elementProperty name="GroupInstances" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="groupInstances" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/GroupInstanceCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="HandlerCollection" xmlItemName="handlerCollection" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/HandlerElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="HandlerElement">
      <attributeProperties>
        <attributeProperty name="Folder" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="folder" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="HandlerName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="handlerName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Host" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="HandlerGroup" xmlItemName="handler" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/GroupElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="GroupElement">
      <attributeProperties>
        <attributeProperty name="HandlerName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="handlerName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="RelativeFolder" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="relativeFolder" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="GroupInstanceCollection" xmlItemName="groupInstance" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/GroupInstance" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="GroupInstance">
      <attributeProperties>
        <attributeProperty name="BaseFolder" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="baseFolder" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="GroupName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="groupName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Host" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="HandlerGroupCollection" xmlItemName="handlerGroup" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementCollectionMoniker name="/1e272c42-8c6a-4f16-9329-58de94f55a53/HandlerGroup" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>