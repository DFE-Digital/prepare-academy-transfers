resource cloudfoundry_app worker_app {
	name               = local.web_app_name
	space              = data.cloudfoundry_space.space.id
	docker_image       = local.docker_image
	strategy           = "blue-green-v2"

	service_binding { 
		service_instance = cloudfoundry_service_instance.redis.id
	}

	service_binding {
		service_instance = cloudfoundry_user_provided_service.logit.id
	}

	routes {
		route = cloudfoundry_route.web_app_cloudapp_digital_route.id
	} 

	environment = {
		"ASPNETCORE_ENVIRONMENT" = var.aspnetcore_environment
		"ASPNETCORE_URLS"        = "http://+:8080"
		"TRAMS_API_BASE"         = var.app_trams_api_base
		"TRAMS_API_KEY"          = var.app_trams_api_key
		"AZUREAD__CLIENTID"		 = var.app_azuread_clientid
		"AZUREAD__TENANTID"		 = var.app_azuread_tenantid
		"USERNAME"               = var.app_username
		"PASSWORD"               = var.app_password
		"SENTRY_RELEASE"         = "academy-transfers-api:${var.cf_app_image_tag}"
	}
}

resource cloudfoundry_route web_app_cloudapp_digital_route {
	domain   = data.cloudfoundry_domain.london_cloud_apps_digital.id
	space    = data.cloudfoundry_space.space.id
	hostname = local.web_app_name
}

resource cloudfoundry_service_instance redis {
	name         = local.redis_service_name
	space        = data.cloudfoundry_space.space.id
	service_plan = data.cloudfoundry_service.redis.service_plans[var.cf_redis_service_plan]
}

resource cloudfoundry_user_provided_service logit {
	name             = local.logit_service_name
	space            = data.cloudfoundry_space.space.id
	syslog_drain_url = var.logit_sink_url
}