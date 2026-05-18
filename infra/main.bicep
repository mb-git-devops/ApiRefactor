@description('Short name used to compose every resource name, e.g. waves-dev')
param namePrefix string

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Environment tag (dev / prod)')
@allowed([
  'dev'
  'prod'
])
param environmentName string

@description('App Service Plan SKU (e.g. B1 for dev, P0v3 / P1v3 for prod)')
param appServicePlanSku string = 'B1'

@description('Service Bus SKU')
@allowed([
  'Basic'
  'Standard'
  'Premium'
])
param serviceBusSku string = 'Standard'

@description('Bearer token the DummyBearer auth handler expects. Override per environment.')
@secure()
param dummyAuthBearerToken string

var planName = '${namePrefix}-plan'
var siteName = '${namePrefix}-api'
var serviceBusNamespaceName = '${namePrefix}-sb'
var serviceBusQueueName = 'wave-upserted'

var commonTags = {
  environment: environmentName
}

module appServicePlan 'modules/appServicePlan.bicep' = {
  name: 'appServicePlan'
  params: {
    name: planName
    location: location
    sku: appServicePlanSku
    tags: commonTags
  }
}

module serviceBus 'modules/serviceBus.bicep' = {
  name: 'serviceBus'
  params: {
    namespaceName: serviceBusNamespaceName
    location: location
    sku: serviceBusSku
    queueName: serviceBusQueueName
    tags: commonTags
  }
}

module appService 'modules/appService.bicep' = {
  name: 'appService'
  params: {
    name: siteName
    location: location
    appServicePlanId: appServicePlan.outputs.id
    appServicePlanSku: appServicePlan.outputs.sku
    environmentName: environmentName
    tags: commonTags
    dummyAuthBearerToken: dummyAuthBearerToken
    serviceBusConnectionString: serviceBus.outputs.primaryConnectionString
  }
}

output appServiceName string = appService.outputs.name
output appServiceHostName string = appService.outputs.defaultHostName
output serviceBusNamespace string = serviceBus.outputs.namespaceName
