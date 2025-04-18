variable "cognito_client_id" {
  type        = string
  description = "The Client ID for the Cognito User Pool."
}

variable "cognito_user_pool_id" {
  type        = string
  description = "The ID of the Cognito User Pool."
}

variable "identity_pool_name" {  # Renamed for clarity
  type        = string
  description = "The name of the Cognito Identity Pool."
}

variable "authenticated_role_arn" {
  type        = string
  description = "The ARN of the default authenticated IAM role."
}

variable "cidm_groups" {
  type = list(object({
    cidm_group = string
    role_arn   = string
  }))
  description = "A list of CIDM groups and corresponding IAM role ARNs."
}