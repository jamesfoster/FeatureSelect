@echo off

echo.
echo === TEST ===
echo.
dotnet test --configuration Release src/FeatureSelect.Tests/FeatureSelect.Tests.csproj

echo.
echo === PACKAGE ===
echo.
set /p version=<version.txt

dotnet pack --include-symbols --include-source --configuration Release --output ..\.. -p:PackageVersion=%version% ^
  src\FeatureSelect\FeatureSelect.csproj
dotnet pack --include-symbols --include-source --configuration Release --output ..\.. -p:PackageVersion=%version% ^
  src\FeatureSelect.Configuration\FeatureSelect.Configuration.csproj
dotnet pack --include-symbols --include-source --configuration Release --output ..\.. -p:PackageVersion=%version% ^
  src\FeatureSelect.SimpleInjector\FeatureSelect.SimpleInjector.csproj

echo.
pause