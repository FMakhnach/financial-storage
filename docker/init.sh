docker volume create --name=db-data

docker build -t financial-storage-api ../FinancialStorage.Api
docker build -t financial-storage-background-workers ../FinancialStorage.BackgroundWorkers
docker build -t financial-storage-migration ../FinancialStorage.Migrations

docker-compose up -d
