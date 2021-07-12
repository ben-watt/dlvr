# We strongly recommend using the required_providers block to set the
# Azure Provider source and version being used
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.46.0"
    }
  }
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
  subscription_id = "31bbb20e-79db-4cc6-a446-c1037b5539cb"
}

resource "azurerm_resource_group" "messaging-sidecar" {
  name     = "messaging-sidecar"
  location = "West Europe"
}

resource "azurerm_servicebus_namespace" "messaging-sidecar" {
  name                = "messaging-sidecar-servicebus-ns"
  location            = azurerm_resource_group.messaging-sidecar.location
  resource_group_name = azurerm_resource_group.messaging-sidecar.name
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}

resource "azurerm_servicebus_topic" "messaging-sidecar" {
  name                = "messaging-sidecar-topic"
  resource_group_name = azurerm_resource_group.messaging-sidecar.name
  namespace_name      = azurerm_servicebus_namespace.messaging-sidecar.name
  enable_partitioning = true
}

resource "azurerm_servicebus_subscription" "messaging-sidecar" {
  name                = "messaging-sidecar-subscription"
  resource_group_name = azurerm_resource_group.messaging-sidecar.name
  namespace_name      = azurerm_servicebus_namespace.messaging-sidecar.name
  topic_name          = azurerm_servicebus_topic.messaging-sidecar.name
  max_delivery_count  = 1
}

resource "azurerm_servicebus_topic_authorization_rule" "messaging-sidecar" {
  name                = "sidecar-account"
  namespace_name      = azurerm_servicebus_namespace.messaging-sidecar.name
  topic_name          = azurerm_servicebus_topic.messaging-sidecar.name
  resource_group_name = azurerm_resource_group.messaging-sidecar.name
  listen              = true
  send                = true
  manage              = false
}

output "connection_string" {
  value = azurerm_servicebus_topic_authorization_rule.messaging-sidecar.primary_connection_string
  description = "Connection string for accessing service bus"
  sensitive = true
}