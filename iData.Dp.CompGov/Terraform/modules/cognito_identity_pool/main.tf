
resource "aws_cognito_identity_pool" "identity_pool" {
  identity_pool_name               = var.identity_pool_name  # Using the updated variable name
  allow_unauthenticated_identities = false

  cognito_identity_providers {
    client_id     = var.cognito_client_id
    provider_name = "cognito-idp.eu-central-1.amazonaws.com/${var.cognito_user_pool_id}"
  }
}


resource "aws_cognito_identity_pool_roles_attachment" "roles" {
  identity_pool_id = aws_cognito_identity_pool.identity_pool.id

  roles = {
    authenticated = var.authenticated_role_arn
  }

  role_mapping {
    identity_provider         = "cognito-idp.eu-central-1.amazonaws.com/${var.cognito_user_pool_id}:${var.cognito_client_id}"
    ambiguous_role_resolution = "AuthenticatedRole"
    type                      = "Rules"

    dynamic "mapping_rule" {
      for_each = var.cidm_groups
      content {
        claim      = "custom:groups"
        match_type = "Contains"
        value      = "\"${mapping_rule.value["cidm_group"]}\""
        role_arn   = mapping_rule.value["role_arn"]
      }
    }
  }
}