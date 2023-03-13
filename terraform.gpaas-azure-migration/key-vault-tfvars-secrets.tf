module "azurerm_key_vault" {
  source = "github.com/DFE-Digital/terraform-azurerm-key-vault-tfvars?ref=v0.1.1"

  environment                           = local.environment
  project_name                          = local.project_name
  resource_group_name                   = module.azure_container_apps_hosting.azurerm_resource_group_default.name
  azure_location                        = local.azure_location
  key_vault_access_users                = local.key_vault_access_users
  tfvars_filename                       = local.tfvars_filename
  diagnostic_log_analytics_workspace_id = module.azure_container_apps_hosting.azurerm_log_analytics_workspace_container_app.id
  diagnostic_eventhub_name              = local.enable_event_hub ? module.azure_container_apps_hosting.azurerm_eventhub_container_app.name : ""
  tags                                  = local.tags
}
