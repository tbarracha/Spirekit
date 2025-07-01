# update-spirecli.ps1

# Step 1: Build NuGet package
dotnet pack -c Release

# Step 2: Update the global tool from the local package
dotnet tool update --global --add-source ./bin/Release SpireCLI

# Step 3: Verify installation
spire help
