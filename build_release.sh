#!/bin/bash

set -e

RELEASE_DIR="./release"
MOD_VERSIONS=("1.0" "1.1" "1.2" "1.3" "1.4" "1.5" "1.6")

rm -rf "$RELEASE_DIR"
mkdir -p "$RELEASE_DIR"

# About, LICENSE und README.md kopieren
cp -r About "$RELEASE_DIR/"
cp LICENSE "$RELEASE_DIR/"
cp README.md "$RELEASE_DIR/"

# DLLs für jede Version kopieren
for v in "${MOD_VERSIONS[@]}"; do
    if [ -f "$v/Assemblies/HighQualityTextures.dll" ]; then
        mkdir -p "$RELEASE_DIR/$v/Assemblies"
        cp "$v/Assemblies/HighQualityTextures.dll" "$RELEASE_DIR/$v/Assemblies/"
    fi
done

echo "Release-Build fertig im Ordner $RELEASE_DIR"