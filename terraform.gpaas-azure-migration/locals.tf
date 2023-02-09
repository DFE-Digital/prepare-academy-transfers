locals {
  environment                            = var.environment
  project_name                           = var.project_name
  azure_location                         = var.azure_location
  tags                                   = var.tags
  virtual_network_address_space          = var.virtual_network_address_space
  enable_container_registry              = var.enable_container_registry
  image_name                             = var.image_name
  container_command                      = var.container_command
  container_secret_environment_variables = var.container_secret_environment_variables
  enable_cdn_frontdoor                   = var.enable_cdn_frontdoor
  cdn_frontdoor_enable_rate_limiting     = var.cdn_frontdoor_enable_rate_limiting
  key_vault_access_users                 = toset(var.key_vault_access_users)
  tfvars_filename                        = var.tfvars_filename
  container_health_probe_path            = var.container_health_probe_path
  cdn_frontdoor_health_probe_path        = var.cdn_frontdoor_health_probe_path
  enable_monitoring                      = var.enable_monitoring
  monitor_email_receivers                = var.monitor_email_receivers
  monitor_endpoint_healthcheck           = var.monitor_endpoint_healthcheck
}
