# SharpExifTool

ExifTool CLI Wrapper in C#

## Development

Before starting development, install 3rd party dependencies by executing `getlibs.sh` (it'll only work on macOS or unix operating system, no Windows script for now).

```bash
$ ./getlibs.sh
```

This will download and extract files based on `.gitbinmodules` content and place them under `libs` directory.

To use different version of `exiftool`, you can edit `.gitbinmodules` file and change it with your desired version.

You can also download from [official website](https://exiftool.org) and extract the files manually. Put them in `libs` directory so it'll look something like this:

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
