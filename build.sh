#!/bin/bash

PROJECT_PATH="./src/Atem/Atem.csproj"
OUTPUT_DIR="./dist"
LICENSE_FILE="./LICENSE"
NOTICE_FILE="./THIRD-PARTY-LICENSES"

declare -A RUNTIMES
RUNTIMES["win-x64"]="cimgui.dll openal.dll SDL2.dll"
RUNTIMES["linux-x64"]="libcimgui.so libopenal.so libSDL2-2.0.so.0"

# check for dotnet
if ! command -v dotnet &> /dev/null; then
    echo "dotnet could not be found. Please install it."
    exit 1
fi

# grab version from .csproj
VERSION=$(grep -oPm1 "(?<=<InformationalVersion>)(.*)(?=</InformationalVersion>)" "$PROJECT_PATH")
if [ -z "$VERSION" ]; then
  VERSION="unknown"
  echo "Warning: Version not found in $PROJECT_PATH. Using 'unknown'."
fi

OUTPUT_PATH_PREFIX="$OUTPUT_DIR/Atem-v$VERSION"

# clean output directory
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

move_runtime_files() {
  local runtime_file_list=$1
  local source_directory=$2
  local destination_directory=$3

  mkdir -p "$destination_directory"
  for runtime_file in $runtime_file_list; do
    if mv "$source_directory/$runtime_file" "$destination_directory/$runtime_file"; then
      echo "Moved $runtime_file"
    else
      echo "Error moving $runtime_file"
      exit 1
    fi
  done
}

copy_license_files() {
  local output_path=$1

  local license_path="$output_path/licenses"
  mkdir -p "$license_path"
  if cp "$LICENSE_FILE" "$license_path" && cp "$NOTICE_FILE" "$license_path"; then
    echo "Copied license and third-party licenses"
  else
    echo "Error copying license files"
    exit 1
  fi
}

publish() {
  local runtime=$1
  local self_contained=$2
  local output_path=$3

  echo "Publishing $runtime ($self_contained)"
  if ! dotnet publish "$PROJECT_PATH" -c Release -r "$runtime" --self-contained "$self_contained" \
        -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o "$output_path"; then
    echo "Error publishing $runtime"
    exit 1
  fi
}

for RUNTIME in "${!RUNTIMES[@]}"; do
  RUNTIMES_LIST="${RUNTIMES[$RUNTIME]}"
  
  echo "Publishing $RUNTIME"

  # framework-dependent
  OUTPUT_PATH="$OUTPUT_PATH_PREFIX-$RUNTIME-nd"
  RUNTIME_OUTPUT_PATH="$OUTPUT_PATH/runtimes/$RUNTIME/native"
  publish "$RUNTIME" false "$OUTPUT_PATH"
  move_runtime_files "$RUNTIMES_LIST" "$OUTPUT_PATH" "$RUNTIME_OUTPUT_PATH"
  copy_license_files "$OUTPUT_PATH"

  # self-contained
  OUTPUT_PATH="$OUTPUT_PATH_PREFIX-$RUNTIME-sc"
  RUNTIME_OUTPUT_PATH="$OUTPUT_PATH/runtimes/$RUNTIME/native"
  publish "$RUNTIME" true "$OUTPUT_PATH"
  move_runtime_files "$RUNTIMES_LIST" "$OUTPUT_PATH" "$RUNTIME_OUTPUT_PATH"
  copy_license_files "$OUTPUT_PATH"
done

echo "Published to $OUTPUT_DIR"