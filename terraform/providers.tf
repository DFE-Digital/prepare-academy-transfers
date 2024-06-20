provider "azurerm" {
  features {}
  skip_provider_registration = true
  storage_use_azuread        = true
  client_id                  = var.azure_client_id
  client_secret              = var.azure_client_secret
  tenant_id                  = var.azure_tenant_id
  subscription_id            = var.azure_subscription_id
}

provider "azapi" {
  enable_hcl_output_for_data_source = true
}

provider "statuscake" {
  api_token = var.statuscake_api_token
}
