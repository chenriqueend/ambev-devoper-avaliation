#!/bin/bash

# Build and start the containers
echo "Building and starting containers..."
sudo docker compose up -d --build

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
sleep 30

# Run database migrations
echo "Running database migrations..."
sudo docker compose exec api dotnet ef database update --project src/Ambev.DeveloperEvaluation.Infrastructure

echo "Application is running!"
echo "API: http://localhost:5000"
echo "Swagger: http://localhost:5000/swagger" 