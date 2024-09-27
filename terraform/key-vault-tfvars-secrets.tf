module "azurerm_key_vault" {
  source = "github.com/DFE-Digital/terraform-azurerm-key-vault-tfvars?ref=v0.5.0"

  environment                             = local.environment
  project_name                            = local.project_name
  existing_resource_group                 = module.azure_container_apps_hosting.azurerm_resource_group_default.name
  azure_location                          = local.azure_location
  key_vault_access_use_rbac_authorization = true
  key_vault_access_users                  = []
  key_vault_access_ipv4                   = local.key_vault_access_ipv4
  tfvars_filename                         = local.tfvars_filename
  diagnostic_log_analytics_workspace_id   = module.azure_container_apps_hosting.azurerm_log_analytics_workspace_container_app.id
  diagnostic_eventhub_name                = local.enable_event_hub ? module.azure_container_apps_hosting.azurerm_eventhub_container_app.name : ""
  tags                                    = local.tags
}
