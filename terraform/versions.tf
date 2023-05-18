terraform {
  required_version = ">= 1.4.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.52.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = ">= 1.5.0"
    }
  }
}
