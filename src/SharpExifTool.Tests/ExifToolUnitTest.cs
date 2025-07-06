using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpExifTool.Tests;


/***
  * Sample images: https://exiftool.org/sample_images.html
  */
public class ExifToolUnitTest
{
    string GetTestPath(string relativePath)
    {
        var codeBasePath = Assembly.GetExecutingAssembly().Location;
        var dirPath = Path.GetDirectoryName(codeBasePath);

        if (dirPath == null)
            throw new InvalidOperationException($"Directory path {dirPath} could not be determined.");

        return Path.Combine(dirPath, "Images", relativePath);
    }

    [Fact]
    public async void ExecuteAsyncTest()
    {
        var images = Directory.GetFiles(GetTestPath("Research In Motion"));

        using var exiftool = new SharpExifTool.ExifTool();
        foreach (var image in images)
        {
            var meta = await exiftool.ExtractAllMetadataAsync(image);
            Assert.True(meta != null && meta.Count > 0);

            // With additional args -u (Unknown tags)
            meta = await exiftool.ExtractAllMetadataAsync(image, "-u");
            Assert.True(meta != null && meta.Count > 0);
        }
    }

    [Fact]
    public async Task ExecuteAsyncTestWithCustomExifToolPath()
    {
        var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (string.IsNullOrWhiteSpace(currentDir))
            throw new InvalidOperationException();

        var path =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(currentDir, "ExifTool.Win", "exiftool.exe")
                : Path.Combine(currentDir, "ExifTool.Unix", "exiftool");

        var images = Directory.GetFiles(GetTestPath("Research In Motion"));

        using var exiftool = new SharpExifTool.ExifTool(exiftoolPath: path);
        foreach (var image in images)
        {
            var meta = await exiftool.ExtractAllMetadataAsync(image);
            Assert.True(meta != null && meta.Count > 0);
        }
    }
}
