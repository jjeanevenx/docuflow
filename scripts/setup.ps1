docker compose up -d
Push-Location infrastructure/terraform
terraform init
terraform apply -auto-approve
Pop-Location
