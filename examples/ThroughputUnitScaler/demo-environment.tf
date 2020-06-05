provider "azurerm" {
   features {}
}

resource "random_integer" "randint" {
  min     = 1000
  max     = 50000
}

resource "azurerm_resource_group" "example" {
  name     = "rg-performancetest${random_integer.randint.result}"
  location = "west europe"

}

resource "azurerm_eventhub_namespace" "example" {
  name                = "performancetestns${random_integer.randint.result}"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  sku                 = "Standard"
  capacity            = 1
  auto_inflate_enabled = true
  maximum_throughput_units = 8
}

resource "azurerm_eventhub" "example" {
  name                = "performancetesthub${random_integer.randint.result}"
  namespace_name      = azurerm_eventhub_namespace.example.name
  resource_group_name = azurerm_resource_group.example.name
  partition_count     = 8
  message_retention   = 1

}

resource "azurerm_storage_account" "example" {
  name                     = "functionsperftestsa${random_integer.randint.result}"
  resource_group_name      = azurerm_resource_group.example.name
  location                 = azurerm_resource_group.example.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "example" {
  name                = "functionperftest${random_integer.randint.result}"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "example" {
  name                      = "AzureFunctionEventGenerator"
  location                  = azurerm_resource_group.example.location
  resource_group_name       = azurerm_resource_group.example.name
  app_service_plan_id       = azurerm_app_service_plan.example.id
  storage_account_name      = azurerm_storage_account.example.name
  storage_account_access_key = azurerm_storage_account.example.primary_access_key
}

output "cosmosdb_connectionstrings" {
   value = azurerm_eventhub_namespace.example.default_primary_connection_string
   sensitive   = true
}