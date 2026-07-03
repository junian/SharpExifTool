<p align="center"><img src="https://raw.githubusercontent.com/junian/SharpExifTool/master/assets/img/sharp-exiftool-icon.png" alt="SharpExifTool Logo"></p>

<h1 align="center">SharpExifTool</h1>

<p align="center">C# Wrapper for ExifTool by Phil Harvey. Available for Windows, macOS, and Linux.</p>

<p align="center">
    <a href="https://github.com/junian/SharpExifTool/"><img src="https://img.shields.io/badge/GitHub-%23121011.svg?logo=github&logoColor=white&style=for-the-badge" alt="SharpExifTool on GitHub" title="SharpExifTool on GitHub"></a>
    <a href="https://www.nuget.org/packages/SharpExifTool/"><img src="https://img.shields.io/nuget/v/SharpExifTool.svg?style=for-the-badge" alt="SharpExifTool latest version on NuGet" title="SharpExifTool latest version on NuGet"></a>
    <a href="https://www.nuget.org/packages/SharpExifTool/"><img src="https://img.shields.io/nuget/dt/SharpExifTool.svg?style=for-the-badge" alt="SharpExifTool total downloads on NuGet" title="SharpExifTool total downloads on NuGet"></a>
</p>

----

## Installation

To install this library to your .NET project, get it the [SharpExifTool](https://www.nuget.org/packages/SharpExifTool/) from NuGet.
You can use the Visual Studio package manager UI or with command line below:

```shell
dotnet add package SharpExifTool
```

## Usage and Examples

In this section, you'll learn on how to use the library properly.
I make the functions as simple as possible.
Hopefully, it can be easily understood by looking at some code examples.

### Init

To use `SharpExifTool`, the first thing you need is to initialize the library.
You can create an exiftool instance by using the built-in `exiftool` binary that is shipped together with `SharpExifTool` package.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    // Write your code here
}
```

The `ExifTool` implements `IDisposable` interface.
Make sure you you call `Dispose()` after finished using it or wrapped it with `using` statement.

### Read All EXIF / Metadata Tags

You can get all metadata from an image by calling the `ExtractAllMetadataAsync` function.
It'll return a collection of `KeyValuePair` of `string`.
Here's an example on how to use this function:

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    var metadataList = await exiftool.ExtractAllMetadataAsync(filename: "image.jpg");
}
```

### Write EXIF / Metadata Tags

You can write a metadata by using `WriteTagsAsync` function.
In this code example, I wrote the `artist` metadata with `Phil Harvey` value to the `image.jpg` without overwriting the original file.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.WriteTagsAsync(
        filename: "image.jpg", 
        properties: new Dictionary<string, string>
        {
            ["artist"] = ["Phil Harvey"],    
        },
        overwriteOriginal: false);
}
```

### Remove All Safe EXIF / Metadata Tags

Let's say you want to strip metadata from user uploaded image reduce the number of Personally Identifiable Information from image metadata, you can call the `RemoveAllMetadataAsync` function.
This will remove all metadata that can be safely removed from an image.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.RemoveAllMetadataAsync(filename: "image.jpg");
}
```

### Custom Command

You can use custom command using this library.

For example, if you already have some predefined command line parameters using the `exiftool` cli like this:

```shell
exiftool -artist="Phil Harvey" -copyright="2011 Phil Harvey" a.jpg
```

You can implement it using this library in C# like this:

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

Before starting development, install 3rd party dependencies by executing `getlibs.sh` (it'll only work on macOS or Unix operating system, no Windows script for now).

```bash
$ ./getlibs.sh
```

This will download and extract files based on `.gitbinmodules` content and place them under the `libs` directory.

To use a different version of `exiftool`, you can edit the `.gitbinmodules` file and change it with your desired version.

You can also download from [official website](https://exiftool.org) and extract the files manually. Put them in the `libs` directory so it'll look something like this:

```shell
.
├── docs
│   └── README.md
├── libs
│   ├── ExifTool.Unix
│   │   ├── Makefile.PL
│   │   ├── arg_files
│   │   ├── build_geolocation
│   │   ├── config_files
│   │   ├── exiftool
│   │   ├── fmt_files
│   │   ├── lib
│   │   ├── perl-Image-ExifTool.spec
│   │   └── t
│   └── ExifTool.Win
│       └── exiftool.exe
└── src
```

## Credits

- [ExifTool](https://exiftool.org) by Phil Harvey.
- [FileMeta/ExifToolWrapper](https://github.com/FileMeta/ExifToolWrapper): CodeBit: C# Wrapper for Phil Harvey's ExifTool
  
## License

- This [SharpExifTool](https://github.com/junian/SharpExifTool) project is licensed under [MIT License](https://github.com/junian/SharpExifTool/blob/master/LICENSE).
- The [ExifTool](https://exiftool.org/#license) is free software; you can redistribute it and/or modify it under the same terms as [Perl itself](https://dev.perl.org/licenses/).

---

Made with ☕ by [Junian.dev](https://www.junian.dev).
