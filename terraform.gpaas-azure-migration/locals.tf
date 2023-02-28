locals {
  environment                                  = var.environment
  project_name                                 = var.project_name
  azure_location                               = var.azure_location
  tags                                         = var.tags
  virtual_network_address_space                = var.virtual_network_address_space
  enable_container_registry                    = var.enable_container_registry
  image_name                                   = var.image_name
  container_command                            = var.container_command
  container_secret_environment_variables       = var.container_secret_environment_variables
  enable_cdn_frontdoor                         = var.enable_cdn_frontdoor
  cdn_frontdoor_enable_rate_limiting           = var.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_threshold        = var.cdn_frontdoor_rate_limiting_threshold
  key_vault_access_users                       = toset(var.key_vault_access_users)
  tfvars_filename                              = var.tfvars_filename
  container_health_probe_path                  = var.container_health_probe_path
  cdn_frontdoor_health_probe_path              = var.cdn_frontdoor_health_probe_path
  enable_monitoring                            = var.enable_monitoring
  monitor_email_receivers                      = var.monitor_email_receivers
  monitor_endpoint_healthcheck                 = var.monitor_endpoint_healthcheck
  monitor_enable_slack_webhook                 = var.monitor_enable_slack_webhook
  monitor_slack_webhook_receiver               = var.monitor_slack_webhook_receiver
  monitor_slack_channel                        = var.monitor_slack_channel
  existing_network_watcher_name                = var.existing_network_watcher_name
  existing_network_watcher_resource_group_name = var.existing_network_watcher_resource_group_name
}
