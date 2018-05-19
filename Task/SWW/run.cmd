cd %~dp0
dotnet restore
dotnet build
dotnet publish -c Release
dotnet SWW.GStats.Server\bin\Release\netcoreapp2.0\publish\SWW.GStats.Server.dll --urls "http://localhost:8080"
