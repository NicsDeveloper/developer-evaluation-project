#!/bin/bash

dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool

dotnet restore
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore

dotnet test Ambev.DeveloperEvaluation.sln --no-restore --verbosity normal \
/p:CollectCoverage=true \
/p:CoverletOutputFormat=cobertura \
/p:CoverletOutput=./TestResults/coverage.cobertura.xml \
/p:Exclude="[*]*.Program,[*]*.Startup,[*]*.Migrations.*"

reportgenerator \
-reports:"./tests/**/TestResults/coverage.cobertura.xml" \
-targetdir:"./TestResults/CoverageReport" \
-reporttypes:Html

rm -rf bin obj

echo ""
echo "Coverage report generated at TestResults/CoverageReport/index.html"
