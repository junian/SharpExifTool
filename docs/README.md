<p align="center"><img src="https://raw.githubusercontent.com/junian/SharpExifTool/master/assets/img/sharp-exiftool-icon.png" alt="SharpExifTool Logo"></p>

<h1 align="center">SharpExifTool</h1>

<p align="center">C# Wrapper for ExifTool by Phil Harvey. Available for Windows, macOS, and Linux.</p>

<p align="center">
    <a href="https://github.com/junian/SharpExifTool/"><img src="https://img.shields.io/badge/GitHub-%23121011.svg?logo=github&logoColor=white&style=for-the-badge" alt="SharpExifTool on GitHub" title="SharpExifTool on GitHub"></a>
    <a href="https://www.nuget.org/packages/SharpExifTool/"><img src="https://img.shields.io/nuget/v/SharpExifTool.svg?style=for-the-badge" alt="SharpExifTool latest version on NuGet" title="SharpExifTool latest version on NuGet"></a>
    <a href="https://www.nuget.org/packages/SharpExifTool/"><img src="https://img.shields.io/nuget/dt/SharpExifTool.svg?style=for-the-badge" alt="SharpExifTool total downloads on NuGet" title="SharpExifTool total downloads on NuGet"></a>
</p>

----

## Overview

`SharpExifTool` is a .NET library that wraps the popular `exiftool` command-line application, making it easier for .NET developers to work with image metadata.

This package ships with the `exiftool` binary, so you don't need to install `exiftool` separately.

## Installation

To install this library in your .NET project, get [SharpExifTool](https://www.nuget.org/packages/SharpExifTool/) from NuGet.
You can use the Visual Studio Package Manager UI or the command line below:

```shell
dotnet add package SharpExifTool
```

## Usage and Examples

In this section, you'll learn how to use the library properly.
I've made the functions as simple as possible.
Hopefully, they can be easily understood by looking at the code examples.

### Init

To use `SharpExifTool`, the first thing you need to do is initialize the library.
You can create an `ExifTool` instance using the built-in `exiftool` binary that is shipped with the `SharpExifTool` package.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    // Write your code here
}
```

The `ExifTool` class implements the `IDisposable` interface.
Make sure you call `Dispose()` after you're finished using it, or wrap it in a `using` statement.

### Read All EXIF / Metadata Tags

You can get all metadata from an image by calling the `ExtractAllMetadataAsync` function.
It returns a collection of `KeyValuePair<string, string>`.
Here's an example of how to use this function:

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    var metadataList = await exiftool.ExtractAllMetadataAsync(filename: "image.jpg");
}
```

### Write EXIF / Metadata Tags

You can write metadata using the `WriteTagsAsync` function.
In this example, the `artist` metadata is set to `Phil Harvey` in `image.jpg` without overwriting the original file.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.WriteTagsAsync(
        filename: "image.jpg",
        properties: new Dictionary<string, string>
        {
            ["artist"] = "Phil Harvey",
        },
        overwriteOriginal: false);
}
```

### Remove All Safe EXIF / Metadata Tags

Let's say you want to strip metadata from a user-uploaded image to reduce the amount of Personally Identifiable Information (PII) stored in its metadata.
You can call the `RemoveAllMetadataAsync` function.
This removes all metadata that can be safely removed from an image.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.RemoveAllMetadataAsync(filename: "image.jpg");
}
```

### Custom Command

You can also execute custom commands using this library.

For example, if you already have predefined `exiftool` CLI parameters like this:

```shell
exiftool -artist="Phil Harvey" -copyright="2011 Phil Harvey" a.jpg
```

You can implement the same command in C# like this:

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.ExecuteAsync(
        "-artist=\"Phil Harvey\"",
        "-copyright=\"2011 Phil Harvey\"",
        "a.jpg");
}
```

## Development

Before starting development, install the third-party dependencies by running `getlibs.sh` (it currently works only on macOS and Unix-like operating systems; there is no Windows script yet).

```bash
$ ./getlibs.sh
```

This downloads and extracts the files defined in `.gitbinmodules` and places them in the `libs` directory.

To use a different version of `exiftool`, edit the `.gitbinmodules` file and change it to your desired version.

Alternatively, you can download it from the [official website](https://exiftool.org) and extract the files manually. Place them in the `libs` directory so the structure looks like this:

```shell
.
├── docs
│   └── README.md
├── libs
│   ├── ExifTool.Unix
│   │   ├── Makefile.PL
│   │   ├── arg_files
│   │   ├── build_geolocation
│   │   ├── config_files
│   │   ├── exiftool
│   │   ├── fmt_files
│   │   ├── lib
│   │   ├── perl-Image-ExifTool.spec
│   │   └── t
│   └── ExifTool.Win
│       └── exiftool.exe
└── src
```

## Credits

- [ExifTool](https://exiftool.org) by Phil Harvey.
- [FileMeta/ExifToolWrapper](https://github.com/FileMeta/ExifToolWrapper): CodeBit C# wrapper for Phil Harvey's ExifTool.

## License

- This [SharpExifTool](https://github.com/junian/SharpExifTool) project is licensed under the [MIT License](https://github.com/junian/SharpExifTool/blob/master/LICENSE).
- [ExifTool](https://exiftool.org/#license) is free software; you can redistribute it and/or modify it under the same terms as [Perl itself](https://dev.perl.org/licenses/).

---

Made with ☕ by [Junian.dev](https://www.junian.dev).
