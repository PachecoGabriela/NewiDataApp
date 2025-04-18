variable "default_role_name" {
  type        = string
  description = "The name prefix for the default IAM role associated with the Cognito Identity Pool."
}

variable "identity_pool_id" {
  type        = string
  description = "The ID of the Cognito Identity Pool."
}

variable "iam_roles_with_redshift_data_api_access" {
  type        = list(string)
  description = "List of IAM roles with Redshift Data API access."
}
