@description('App Service Plan name')
param name string

@description('Azure region')
param location string

@description('App Service Plan SKU (e.g. B1, P0v3, P1v3)')
param sku string

@description('Tags applied to the plan')
param tags object = {}

resource servicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: name
  location: location
  sku: {
    name: sku
  }
  kind: 'app'
  properties: {
    reserved: false
  }
  tags: tags
}

output id string = servicePlan.id
output name string = servicePlan.name
output sku string = sku
