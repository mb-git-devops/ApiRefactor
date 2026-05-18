@description('App Service (web app) name')
param name string

@description('Azure region')
param location string

@description('Resource ID of the App Service Plan this app runs on')
param appServicePlanId string

@description('App Service Plan SKU — used to decide whether AlwaysOn can be enabled')
param appServicePlanSku string

@description('Environment tag (dev / prod). Drives ASPNETCORE_ENVIRONMENT.')
@allowed([
  'dev'
  'prod'
])
param environmentName string

@description('Tags applied to the site')
param tags object = {}

@secure()
@description('Bearer token consumed by the DummyBearer auth handler')
param dummyAuthBearerToken string

@secure()
@description('Service Bus namespace connection string')
param serviceBusConnectionString string

resource site 'Microsoft.Web/sites@2023-12-01' = {
  name: name
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      alwaysOn: appServicePlanSku != 'F1' && appServicePlanSku != 'D1'
      ftpsState: 'Disabled'
      http20Enabled: true
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environmentName == 'prod' ? 'Production' : 'Development'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'WaveDatabase__ConnectionString'
          value: 'Data Source=%HOME%\\site\\waves.db'
        }
        {
          name: 'MassTransit__Transport'
          value: 'AzureServiceBus'
        }
        {
          name: 'MassTransit__AzureServiceBusConnectionString'
          value: serviceBusConnectionString
        }
        {
          name: 'DummyAuth__BearerToken'
          value: dummyAuthBearerToken
        }
      ]
    }
  }
  tags: tags
}

output name string = site.name
output defaultHostName string = site.properties.defaultHostName
