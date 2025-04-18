resource "aws_iam_role" "default_identity_pool_role" {
  name               = "${var.default_role_name}_default_role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Principal = {
          Federated = "cognito-identity.amazonaws.com"
        },
        Action = "sts:AssumeRoleWithWebIdentity",
        Condition = {
          StringEquals = {
            "cognito-identity.amazonaws.com:aud": var.identity_pool_id
          },
          "ForAnyValue:StringLike": {
            "cognito-identity.amazonaws.com:amr": "authenticated"
          }
        }
      }
    ]
  })

  description = "Default IAM role for the Cognito Identity Pool"
}

resource "aws_iam_role_policy" "default_identity_pool_policy" {
  name = "${var.default_role_name}_default_policy"
  role = aws_iam_role.default_identity_pool_role.id

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "cognito-identity:GetCredentialsForIdentity"
        ],
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role" "endpointreadonlyrole" {
  for_each = toset(var.iam_roles_with_redshift_data_api_access)

  name               = "${each.value}_readonly_role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Principal = {
          Federated = "cognito-identity.amazonaws.com"
        },
        Action = "sts:AssumeRoleWithWebIdentity",
        Condition = {
          StringEquals = {
            "cognito-identity.amazonaws.com:aud": var.identity_pool_id
          }
        }
      }
    ]
  })

  description = "IAM Role for Cognito Identity Pool Endpoint ${each.value}"
}

data "aws_iam_policy" "idata_redshift_data_api_access" {
  name = "idata_redshift_data_api_access"
}

resource "aws_iam_role_policy_attachment" "policy_attach" {
  for_each  = toset(var.iam_roles_with_redshift_data_api_access)
  role      = aws_iam_role.endpointreadonlyrole[each.key].name
  policy_arn = data.aws_iam_policy.idata_redshift_data_api_access.arn
}
