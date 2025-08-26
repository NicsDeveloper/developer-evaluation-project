@ECHO OFF

dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool

dotnet restore Ambev.DeveloperEvaluation.sln
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore

dotnet test Ambev.DeveloperEvaluation.sln --no-restore --verbosity normal ^
/p:CollectCoverage=true ^
/p:CoverletOutputFormat=cobertura ^
/p:CoverletOutput=./TestResults/coverage.cobertura.xml ^
/p:Exclude="[*]*.Program%2c[*]*.Startup%2c[*]*.Migrations.*"

reportgenerator ^
-reports:"./tests/**/TestResults/coverage.cobertura.xml" ^
-targetdir:"./TestResults/CoverageReport" ^
-reporttypes:Html

rmdir /s /q bin 2>nul
rmdir /s /q obj 2>nul

echo.
echo Coverage report generated at TestResults/CoverageReport/index.html
pause
