echo Starting Install Script
cd ApiLibs
nuget restore
cd ..
nuget restore
xbuild GHDeploy.sln