#!/bin/bash
set -e

echo "🔧 Building csharpcompile..."

dotnet restore
dotnet build src/Cli

echo "🚀 Publishing self..."

dotnet publish src/Cli -c Release -o out

echo "✅ Done"
