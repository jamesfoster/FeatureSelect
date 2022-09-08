@echo off

pushd %~dp0
pushd src

echo.
echo === TEST ===
echo.
dotnet test  --configuration Release

echo.
echo === PACKAGE ===
echo.
dotnet pack ^
  --include-symbols ^
	 -p:SymbolPackageFormat=snupkg ^
	--include-source ^
	--configuration Release ^
	--output .. ^
	FeatureSelect\FeatureSelect.csproj

popd
popd

echo.
pause