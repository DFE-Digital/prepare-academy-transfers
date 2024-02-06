module "statuscake-tls-monitor" {
  source = "github.com/dfe-digital/terraform-statuscake-tls-monitor?ref=v0.1.2"

  statuscake_monitored_resource_addresses = local.statuscake_monitored_resource_addresses
  statuscake_alert_at = [ # days to alert on
    40, 20, 5
  ]
  statuscake_contact_group_name            = local.statuscake_contact_group_name
  statuscake_contact_group_integrations    = local.statuscake_contact_group_integrations
  statuscake_contact_group_email_addresses = local.statuscake_contact_group_email_addresses
}
