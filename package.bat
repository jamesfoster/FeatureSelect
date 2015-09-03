@echo off

set path=%path%;c:\Windows\Microsoft.NET\Framework64\v4.0.30319

call msbuild src\FeatureSelect\FeatureSelect.csproj /t:rebuild /p:Configuration=Release
call msbuild src\FeatureSelect.Sql\FeatureSelect.Sql.csproj /t:rebuild /p:Configuration=Release

echo.
NuGet pack src\FeatureSelect\FeatureSelect.csproj -Prop Configuration=Release
NuGet pack src\FeatureSelect.Sql\FeatureSelect.Sql.csproj -Prop Configuration=Release

echo.
pause