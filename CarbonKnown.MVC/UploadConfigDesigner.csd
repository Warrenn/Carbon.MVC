<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="9425462d-b534-4c5c-a920-342d45d4931d" namespace="CarbonKnown.MVC" xmlSchemaNamespace="urn:CarbonKnown.MVC" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="UploadConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="uploadConfig">
      <attributeProperties>
        <attributeProperty name="StagingFolder" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="stagingFolder" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="FileTypes" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="fileTypes" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/FileTypes" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="FileTypes" xmlItemName="fileType" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/FileType" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="FileType">
      <attributeProperties>
        <attributeProperty name="Handler" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="handler" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Folder" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="folder" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Description" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="description" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="DisplayName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="displayName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9425462d-b534-4c5c-a920-342d45d4931d/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>