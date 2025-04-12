#!/bin/bash

PROJECT_PATH="./src/Atem/Atem.csproj"
RUNTIMES=("win-x64" "linux-x64")
OUTPUT_DIR="./dist"
LICENSE_FILE="./LICENSE"
NOTICE_FILE="./THIRD-PARTY-LICENSES"

# grab version from .csproj
if ! VERSION=$(grep -oPm1 "(?<=<InformationalVersion>)(.*)(?=</InformationalVersion>)" "$PROJECT_PATH"); then
  VERSION="unknown"
  echo "Warning: Version not found in $PROJECT_PATH. Using 'unknown'."
fi

OUTPUT_PATH_PREFIX="$OUTPUT_DIR/Atem-v$VERSION"

# clean output directory
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

for RUNTIME in "${RUNTIMES[@]}"
do
  echo "Publishing $RUNTIME (framework-dependent)"
  if ! dotnet publish "$PROJECT_PATH" -c Release -r "$RUNTIME" -p:DebugType=None \
        -p:DebugSymbols=false -o "$OUTPUT_PATH_PREFIX-$RUNTIME-nd"; then

    echo "Error publishing framework-dependent release for $RUNTIME"
  fi

  echo "Copying license file."
  cp "$LICENSE_FILE" "$OUTPUT_PATH_PREFIX-$RUNTIME-nd/"
  echo "Copying third party licenses file."
  cp "$NOTICE_FILE" "$OUTPUT_PATH_PREFIX-$RUNTIME-nd/"

  echo "Publishing $RUNTIME (self-contained)"
  if ! dotnet publish "$PROJECT_PATH" -c Release -r "$RUNTIME" --self-contained true \
        -p:PublishSingleFile=true -p:UseAppHost=true -p:DebugType=None -p:DebugSymbols=false \
        -o "$OUTPUT_PATH_PREFIX-$RUNTIME-sc"; then
    
    echo "Error publishing self-contained release for $RUNTIME"
  fi

  echo "Copying license file."
  cp "$LICENSE_FILE" "$OUTPUT_PATH_PREFIX-$RUNTIME-sc/"
  echo "Copying third party licenses file."
  cp "$NOTICE_FILE" "$OUTPUT_PATH_PREFIX-$RUNTIME-sc/"
done

echo "Published to $OUTPUT_DIR"