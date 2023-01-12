module "azure_container_apps_hosting" {
  source = "github.com/DFE-Digital/terraform-azurerm-container-apps-hosting?ref=v0.12.0"

  environment    = local.environment
  project_name   = local.project_name
  azure_location = local.azure_location
  tags           = local.tags

  virtual_network_address_space = local.virtual_network_address_space

  enable_container_registry = local.enable_container_registry

  image_name                             = local.image_name
  container_command                      = local.container_command
  container_secret_environment_variables = local.container_secret_environment_variables

  enable_mssql_database              = local.enable_mssql_database
  enable_redis_cache                 = local.enable_redis_cache
  enable_cdn_frontdoor               = local.enable_cdn_frontdoor
  cdn_frontdoor_enable_rate_limiting = local.cdn_frontdoor_enable_rate_limiting
}
