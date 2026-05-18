@description('Service Bus namespace name')
param namespaceName string

@description('Azure region')
param location string

@description('Service Bus SKU')
@allowed([
  'Basic'
  'Standard'
  'Premium'
])
param sku string = 'Standard'

@description('Queue used for the WaveUpserted integration event')
param queueName string

@description('Tags applied to the namespace')
param tags object = {}

var authRuleName = 'RootManageSharedAccessKey'

resource serviceBus 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: namespaceName
  location: location
  sku: {
    name: sku
    tier: sku
  }
  tags: tags
}

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: serviceBus
  name: queueName
  properties: {
    lockDuration: 'PT30S'
    maxDeliveryCount: 10
    deadLetteringOnMessageExpiration: true
  }
}

resource serviceBusAuthRule 'Microsoft.ServiceBus/namespaces/authorizationRules@2022-10-01-preview' existing = {
  parent: serviceBus
  name: authRuleName
}

output namespaceName string = serviceBus.name
output queueName string = serviceBusQueue.name

@secure()
output primaryConnectionString string = serviceBusAuthRule.listKeys().primaryConnectionString
