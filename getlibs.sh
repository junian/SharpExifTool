#!/bin/sh

set -e

RUNTIMES_DIR="runtimes"
DOWNLOAD_DIR=$(mktemp -d)

cleanup() {
    rm -rf "$DOWNLOAD_DIR"
}
trap cleanup EXIT

mkdir -p "$RUNTIMES_DIR"
if [ -d "$RUNTIMES_DIR" ]; then
    chmod -R u+w "$RUNTIMES_DIR" 2>/dev/null || true
fi
rm -rf "$RUNTIMES_DIR"/*

echo "Downloading files ..."

while IFS= read -r url || [ -n "$url" ]; do
    # Ignore blank lines and comments
    url=$(printf '%s' "$url" | tr -d '\r' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
    case "$url" in
        ''|'#'*)
            continue
            ;;
    esac

    # SourceForge download URLs end with "/download", so use the actual
    # archive name from the URL instead.
    filename=$(basename "$url")
    if [ "$filename" = "download" ]; then
        filename=$(basename "$(dirname "$url")")
    fi

    curl -fL --retry 3 -o "$DOWNLOAD_DIR/$filename" "$url"
done < .gitbinmodules

echo "Download finished."

echo "Extracting files ..."

# Windows x86 (32-bit)
mkdir -p runtimes/win-x86
unzip -o "$DOWNLOAD_DIR"/exiftool-*_32.zip -d runtimes/win-x86
mv runtimes/win-x86/exiftool-*_32 runtimes/win-x86/native
chmod -R u+w runtimes/win-x86/native
mv runtimes/win-x86/native/exiftool*.exe runtimes/win-x86/native/exiftool.exe

# Windows x64 (64-bit)
mkdir -p runtimes/win-x64
unzip -o "$DOWNLOAD_DIR"/exiftool-*_64.zip -d runtimes/win-x64
mv runtimes/win-x64/exiftool-*_64 runtimes/win-x64/native
chmod -R u+w runtimes/win-x64/native
mv runtimes/win-x64/native/exiftool*.exe runtimes/win-x64/native/exiftool.exe

# Unix
mkdir -p runtimes/unix/native
tar -jxvf "$DOWNLOAD_DIR"/Image-ExifTool*.tar.gz -C runtimes/unix/native --strip-components=1

echo "Files extraction finished. You can use them for development now."
