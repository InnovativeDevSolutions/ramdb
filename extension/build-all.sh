#!/bin/bash

configuration="Release"
baseOutputPath="./artifacts"

# Detect platform
if [[ "$OSTYPE" == "darwin"* ]]; then
    currentPlatform="osx-x64"
    libExtension="dylib"
else
    currentPlatform="linux-x64"
    libExtension="so"
fi

echo "Building for current platform: $currentPlatform"

# Create output directories
nativeOutputPath="$baseOutputPath/native"
mkdir -p "$nativeOutputPath"

# Build native shared library
echo "Building native shared library..."
dotnet publish \
    -c $configuration \
    -r $currentPlatform \
    --self-contained true \
    -p:BuildType=lib \
    -p:PublishDir=$nativeOutputPath

if [ $? -ne 0 ]; then
    echo "Library build failed. Check the error messages above."
    exit 1
fi

# Rename the native library to the expected name
dllPath="$nativeOutputPath/ArmaRAMDb.$libExtension"
libDllPath="$nativeOutputPath/ArmaRAMDb_x64.$libExtension"
if [ -f "$dllPath" ]; then
    echo "Renaming native library to ArmaRAMDb_x64.$libExtension..."
    mv "$dllPath" "$libDllPath"
fi

echo "Build complete. Outputs available at:"
echo "Native library: $nativeOutputPath"

# Make the library executable
if [ -f "$libDllPath" ]; then
    echo "Setting executable permissions on $libDllPath"
    chmod +x "$libDllPath"
else
    echo "Warning: Native library not found at $libDllPath"
fi