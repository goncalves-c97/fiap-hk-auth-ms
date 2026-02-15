terraform {
  backend "s3" {
    bucket = "fiap-terraform-backend-infra-tf"
    key    = "hk/auth-ms/terraform.tfstate"
    region = "us-east-1"
  }
}
