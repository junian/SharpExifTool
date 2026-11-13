using SharpExifTool;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: ConsoleDemo <image-path>");
    return 1;
}

var imagePath = args[0];

if (!File.Exists(imagePath))
{
    Console.Error.WriteLine($"File not found: {imagePath}");
    return 1;
}

try
{
    using var exifTool = new ExifTool();
    var metadata = exifTool.ExtractAllMetadata(imagePath, "-a", "-u");

    if (metadata.Count == 0)
    {
        Console.WriteLine("No metadata found.");
        return 0;
    }

    foreach (var item in metadata)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to read metadata: {ex.Message}");
    return 1;
}
