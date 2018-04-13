echo Starting Install Script
cd ApiLibs
dotnet restore
cd ..
dotnet publish ghdeploy.sln
