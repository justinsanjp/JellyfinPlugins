#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$ROOT_DIR/src/SubProfiles/Jellyfin.Plugin.SubProfiles.csproj"
PUBLISH_ROOT="$ROOT_DIR/tmp/publish/SubProfiles"
DIST_DIR="$ROOT_DIR/dist"

rm -rf "$PUBLISH_ROOT"
mkdir -p "$PUBLISH_ROOT"

dotnet publish "$PROJECT" -c Release -o "$PUBLISH_ROOT" /p:DebugType=None

VERSION=$(python3 -c 'import json, sys; print(json.load(open(sys.argv[1]))["version"])' "$ROOT_DIR/src/SubProfiles/plugin.json")
ZIP_NAME="jellyfin.plugin.subprofiles_${VERSION}.zip"

mkdir -p "$DIST_DIR"
rm -f "$DIST_DIR/$ZIP_NAME"

pushd "$PUBLISH_ROOT" >/dev/null
ARTIFACTS=(
  "Jellyfin.Plugin.SubProfiles.dll"
  "Jellyfin.Plugin.SubProfiles.deps.json"
  "Jellyfin.Plugin.SubProfiles.pdb"
  "plugin.json"
)

INCLUDES=()
for artifact in "${ARTIFACTS[@]}"; do
  if [[ -f "$artifact" ]]; then
    INCLUDES+=("$artifact")
  fi
done

if [[ ${#INCLUDES[@]} -eq 0 ]]; then
  echo "No build outputs were found in $PUBLISH_ROOT" >&2
  exit 1
fi

zip -j "$DIST_DIR/$ZIP_NAME" "${INCLUDES[@]}"
popd >/dev/null

sha256sum "$DIST_DIR/$ZIP_NAME"
