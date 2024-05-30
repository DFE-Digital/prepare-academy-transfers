provider "azurerm" {
  features {}
  skip_provider_registration = true
}

provider "azapi" {
  enable_hcl_output_for_data_source = true
}

provider "statuscake" {
  api_token = var.statuscake_api_token
}
