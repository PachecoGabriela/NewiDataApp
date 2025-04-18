terraform {
  backend "s3" {
    region = "eu-central-1"
    max_retries = 1
  }
}