output "service_url" {
  description = "URL publica do microsservico de autenticacao."
  value       = data.terraform_remote_state.infra.outputs.auth_url
}

output "ecs_cluster_name" {
  description = "Cluster ECS compartilhado."
  value       = data.terraform_remote_state.infra.outputs.ecs_cluster_name
}

output "ecs_service_name" {
  description = "Nome do servico ECS de autenticacao."
  value       = data.terraform_remote_state.infra.outputs.ecs_service_names.auth
}

output "db_endpoint" {
  description = "Endpoint do banco SQL Server usado pela autenticacao."
  value       = data.terraform_remote_state.infra.outputs.database_endpoints.auth
}

output "db_secret_arn" {
  description = "ARN do secret com DB_CONNECTION_STRING e DB_NAME."
  value       = data.terraform_remote_state.infra.outputs.database_secret_arns.auth
}

output "shared_secret_arn" {
  description = "ARN do secret compartilhado com API_AUTHENTICATION_KEY."
  value       = data.terraform_remote_state.infra.outputs.shared_secret_arn
}

output "container_image" {
  description = "Imagem Docker Hub configurada para o auth-ms."
  value       = data.terraform_remote_state.infra.outputs.dockerhub_images.auth
}
