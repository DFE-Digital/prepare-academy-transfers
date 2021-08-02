data cloudfoundry_domain london_cloud_apps_digital {
  name = "london.cloudapps.digital"
}

data cloudfoundry_org org {
  name = "dfe"
}

data cloudfoundry_space space {
  name = var.cf_space
  org  = data.cloudfoundry_org.org.id
}

data cloudfoundry_service redis {
  name = "redis"
}