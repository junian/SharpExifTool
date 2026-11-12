#!/bin/sh

set -e

mkdir -p libs
rm -rf libs/*

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

    curl -fL --retry 3 -o "libs/$filename" "$url"
done < .gitbinmodules

echo "Download finished."

echo "Extracting files ..."

# Windows x86 (32-bit)
unzip -o libs/exiftool-*_32.zip -d libs
rm libs/exiftool-*_32.zip
mv libs/exiftool-*_32 libs/ExifTool.Win32
mv libs/ExifTool.Win32/exiftool*.exe libs/ExifTool.Win32/exiftool.exe

# Windows x64 (64-bit)
unzip -o libs/exiftool-*_64.zip -d libs
rm libs/exiftool-*_64.zip
mv libs/exiftool-*_64 libs/ExifTool.Win64
mv libs/ExifTool.Win64/exiftool*.exe libs/ExifTool.Win64/exiftool.exe

# Unix
tar -jxvf libs/Image-ExifTool*.tar.gz -C libs/
rm libs/Image-ExifTool*.tar.gz
mv libs/Image-ExifTool* libs/ExifTool.Unix

echo "Files extraction finished. You can use them for development now."
