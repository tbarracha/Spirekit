param(
    [switch]$major,
    [switch]$minor,
    [switch]$patch
)

# Ensure only one is selected
$selected = @($major, $minor, $patch) | Where-Object { $_ }
if ($selected.Count -ne 1) {
    Write-Host "Usage: .\upgrade-spirecli.ps1 --major | --minor | --patch"
    exit 1
}

$csproj = "../SpireCLI.csproj"
if (!(Test-Path $csproj)) {
    $csproj = "SpireCLI.csproj"
    if (!(Test-Path $csproj)) {
        Write-Error "Could not find SpireCLI.csproj!"
        exit 1
    }
}

# Extract version
[xml]$proj = Get-Content $csproj
$versionNode = $proj.Project.PropertyGroup.Version
if (-not $versionNode) {
    Write-Error "No <Version> tag found in $csproj"
    exit 1
}
$currentVersion = $versionNode.'#text'
if (-not $currentVersion) {
    Write-Error "Current version not found!"
    exit 1
}
Write-Host "Current version: $currentVersion"

$split = $currentVersion.Split('.')
if ($split.Length -ne 3) {
    Write-Error "Version must be in the format X.Y.Z"
    exit 1
}
[int]$majorVer = $split[0]
[int]$minorVer = $split[1]
[int]$patchVer = $split[2]

if ($major) {
    $majorVer++; $minorVer=0; $patchVer=0
} elseif ($minor) {
    $minorVer++; $patchVer=0
} elseif ($patch) {
    $patchVer++
}

$newVersion = "$majorVer.$minorVer.$patchVer"
Write-Host "Updating version to $newVersion"

# Update the XML
$versionNode.'#text' = $newVersion
$proj.Save($csproj)

# Call update script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$updateScript = Join-Path $scriptDir "update-spirecli.ps1"

if (Test-Path $updateScript) {
    & $updateScript
} else {
    Write-Host "update-spirecli.ps1 not found, running update inline..."
    dotnet pack -c Release
    dotnet tool update --global --add-source ./bin/Release SpireCLI
    spire help
}
