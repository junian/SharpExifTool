#!/bin/sh

mkdir -p libs
rm -rf libs/*

echo "Downloading files ..."

wget -P libs -i .gitbinmodules

echo "Download finished."

echo "Extracting files ..."

mkdir -p libs/ExifTool.Win
unzip -o libs/exiftool-*.zip -d libs
rm libs/exiftool-*.zip
mv libs/exiftool*.exe libs/ExifTool.Win/exiftool.exe

mkdir -p libs/
tar -jxvf libs/Image-ExifTool*.tar.gz -C libs/
rm libs/Image-ExifTool*.tar.gz
mv libs/Image-ExifTool* libs/ExifTool.Unix

echo "Files extraction finished. You can use them for development now."

