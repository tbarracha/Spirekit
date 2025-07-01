# uninstall if present
dotnet tool uninstall --global SpireCLI

# build NuGet package
dotnet pack -c Release

# install latest version globally from local package
dotnet tool install --global --add-source ./bin/Release SpireCLI

# verify install
spire help
