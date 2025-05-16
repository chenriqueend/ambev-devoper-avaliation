#!/bin/bash
set -e

echo "🔄 Aplicando migrations..."
dotnet Ambev.DeveloperEvaluation.WebApi.dll --migrate

echo "🚀 Iniciando aplicação..."
exec dotnet Ambev.DeveloperEvaluation.WebApi.dll
