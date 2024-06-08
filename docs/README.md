<p align="center"><img src="https://raw.githubusercontent.com/junian/SharpExifTool/master/assets/img/sharp-exiftool-icon.png" alt="SharpExifTool Logo"></p>

<h1 align="center">SharpExifTool</h1>

<p align="center">C# Wrapper for ExifTool by Phil Harvey. Available for Windows, macOS, and Linux.</p>

<p align="center">
    <a href="https://www.nuget.org/packages/SharpExifTool/"><img src="https://img.shields.io/nuget/v/SharpExifTool.svg" alt="SharpExifTool latest version on NuGet" title="SharpExifTool latest version on NuGet"></a>
    <a href="https://www.nuget.org/packages/SharpExifTool/"><img src="https://img.shields.io/nuget/dt/SharpExifTool.svg" alt="SharpExifTool total downloads on NuGet" title="SharpExifTool total downloads on NuGet"></a>
</p>

----

## Installation

Get [SharpExifTool](https://www.nuget.org/packages/SharpExifTool/) from NuGet.

```powershell
PM> Install-Package SharpExifTool
```

## Usage

### Init

You can create an exiftool instance by using the built-in `exiftool` binary.

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
 // Do something here
}
```

Or if you want to provide your own `exiftool` binary.

```csharp
using(var exiftool = new SharpExifTool.ExifTool(exiftoolPath: "/opt/homebrew/bin/exiftool"))
{
 // Do something here
}
```

### Read All EXIF / Metadata Tags

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.ExtractAllMetadataAsync(filename: "image.jpg");
}
```

### Write EXIF / Metadata Tags

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.WriteTagsAsync(
        filename: "image.jpg", 
        properties: new Dictionary<string, string>
 {
 ["artist"] = ["Phil Harvey"],    
 });
}
```

### Remove All Safe EXIF / Metadata Tags

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.RemoveAllMetadataAsync(filename: "image.jpg");
}
```

### Custom Command

For example, you want to execute the following in the command line:

```shell
exiftool -artist="Phil Harvey" -copyright="2011 Phil Harvey" a.jpg
```

You can do it like this in C#:

```csharp
using(var exiftool = new SharpExifTool.ExifTool())
{
    await exiftool.ExecuteAsync("-artist=\"Phil Harvey\" -copyright=\"2011 Phil Harvey\" a.jpg");
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

This project is licensed under [MIT License](https://github.com/junian/SharpExifTool/blob/master/LICENSE).

---

Made with ☕ by [Junian.dev](https://www.junian.dev).
