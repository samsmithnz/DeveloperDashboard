@description('Globally unique App Service name.')
param appName string

@description('Globally unique, 3-24 character lowercase storage account name.')
@minLength(3)
@maxLength(24)
param storageAccountName string

@description('Azure Table Storage table name.')
param tableName string = 'Snapshots'

param location string = resourceGroup().location

var storageTableDataContributorRoleDefinitionId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'
)
var storageTableEndpoint = 'https://${storageAccount.name}.table.${environment().suffixes.storage}'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${appName}-plan'
  location: location
  kind: 'linux'
  sku: {
    name: 'B1'
    tier: 'Basic'
    capacity: 1
  }
  properties: {
    reserved: true
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowSharedKeyAccess: false
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
}

resource snapshotsTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-05-01' = {
  parent: tableService
  name: tableName
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: appName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      ftpsState: 'Disabled'
      linuxFxVersion: 'DOTNETCORE|10.0'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'AzureTableStorage__Endpoint'
          value: storageTableEndpoint
        }
        {
          name: 'AzureTableStorage__TableName'
          value: tableName
        }
      ]
    }
  }
}

resource tableDataContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: snapshotsTable
  name: guid(snapshotsTable.id, webApp.name, storageTableDataContributorRoleDefinitionId)
  properties: {
    roleDefinitionId: storageTableDataContributorRoleDefinitionId
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output appUrl string = 'https://${webApp.properties.defaultHostName}'
output tableEndpoint string = storageTableEndpoint
output tableName string = tableName
