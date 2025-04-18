output "default_iam_role_arn" {
  value       = aws_iam_role.default_identity_pool_role.arn
  description = "The ARN of the default IAM role created for the Cognito Identity Pool."
}

output "iam_role_arns" {
  value       = { for name in var.iam_roles_with_redshift_data_api_access : name => aws_iam_role.endpointreadonlyrole[name].arn }
  description = "The ARNs of the IAM roles created for each endpoint."
}