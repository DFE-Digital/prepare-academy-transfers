data "azurerm_client_config" "current" {}

data "azuread_user" "key_vault_access" {
  for_each = local.key_vault_access_users

  user_principal_name = each.value
}