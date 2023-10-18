provider "azurerm" {
  features {}
  skip_provider_registration = true
}

provider "statuscake" {
  api_token = var.statuscake_api_token
}
