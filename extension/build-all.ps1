# Build configuration
$configuration = "Release"
$baseOutputPath = ".\artifacts"

# Determine current OS and platform
$currentPlatform = "win-x64" # Since we're running PowerShell, we're on Windows
Write-Host "Building for current platform: $currentPlatform"

# Create output directories
$nativeOutputPath = "$baseOutputPath\native"
New-Item -ItemType Directory -Force -Path $nativeOutputPath | Out-Null

# Build native shared library
Write-Host "Building native shared library..."
dotnet publish `
    -c $configuration `
    -r $currentPlatform `
    --self-contained true `
    -p:BuildType=lib `
    -p:PublishDir=$nativeOutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Library build failed. Check the error messages above."
    exit 1
}

# Rename the native library to the expected name
$dllPath = Join-Path $nativeOutputPath "ArmaRAMDb.dll"
$libDllPath = Join-Path $nativeOutputPath "ArmaRAMDb_x64.dll"
if (Test-Path $dllPath) {
    Write-Host "Renaming native library to ArmaRAMDb_x64.dll..."
    Move-Item -Path $dllPath -Destination $libDllPath -Force
}

Write-Host "Build complete. Outputs available at:"
Write-Host "Native library: $nativeOutputPath"