output "identity_pool_id" {
  value       = aws_cognito_identity_pool.identity_pool.id
  description = "The ID of the created Cognito Identity Pool."
}