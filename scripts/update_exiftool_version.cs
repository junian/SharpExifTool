#!/usr/bin/env dotnet run
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SharpExifTool.Tools;

/// <summary>
/// Checks SourceForge for the latest ExifTool release and always overwrites
/// <c>.gitbinmodules</c> with the matching Windows (32-bit and 64-bit) and Unix
/// download links. Whether anything actually changed is left to the caller.
/// </summary>
public static class ExifToolVersionUpdater
{
    private const string FeedUrl = "https://sourceforge.net/projects/exiftool/rss?path=/";
    private const string GitBinModulesFileName = ".gitbinmodules";
    private const string UserAgent =
        "SharpExifTool-VersionCheck/1.0 (+https://github.com/junian/SharpExifTool)";

    private static readonly Regex Windows32Pattern = new(
        @"^exiftool-(?<version>\d+(?:\.\d+)+)_32\.zip$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex Windows64Pattern = new(
        @"^exiftool-(?<version>\d+(?:\.\d+)+)_64\.zip$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex UnixPattern = new(
        @"^Image-ExifTool-(?<version>\d+(?:\.\d+)+)\.tar\.gz$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// Runs the version check and overwrites <c>.gitbinmodules</c> with the latest release.
    /// </summary>
    /// <param name="args">
    /// Optional path to the repository root. When omitted, the nearest parent directory
    /// containing <c>.gitbinmodules</c> is used.
    /// </param>
    /// <returns>0 when the check completes, 1 when an error occurs.</returns>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var repositoryRoot = ResolveRepositoryRoot(args);
            var gitBinModulesPath = Path.Combine(repositoryRoot, GitBinModulesFileName);

            var feed = await DownloadFeedAsync();
            var latestRelease = FindLatestRelease(feed);

            if (latestRelease is null)
            {
                Console.WriteLine(
                    "No complete release (Windows 32-bit, Windows 64-bit, and Unix archives) " +
                    "was found in the SourceForge feed. Nothing to do.");
                return 0;
            }

            Console.WriteLine($"Latest ExifTool version: {latestRelease.Version}");

            var newLine = File.Exists(gitBinModulesPath) &&
                          File.ReadAllText(gitBinModulesPath).Contains("\r\n", StringComparison.Ordinal)
                ? "\r\n"
                : "\n";

            var updatedContent = string.Join(
                newLine,
                latestRelease.Windows32Url,
                latestRelease.Windows64Url,
                latestRelease.UnixUrl);

            File.WriteAllText(gitBinModulesPath, updatedContent);

            Console.WriteLine($"Wrote {GitBinModulesFileName}:");
            Console.WriteLine($"  {latestRelease.Windows32Url}");
            Console.WriteLine($"  {latestRelease.Windows64Url}");
            Console.WriteLine($"  {latestRelease.UnixUrl}");

            WriteGitHubOutput("version", latestRelease.Version.ToString());
            WriteGitHubSummary(latestRelease);

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Locates the repository root that contains <c>.gitbinmodules</c>.
    /// </summary>
    /// <param name="args">Optional repository root supplied on the command line.</param>
    /// <returns>The absolute path to the repository root.</returns>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown when no directory containing <c>.gitbinmodules</c> can be found.
    /// </exception>
    private static string ResolveRepositoryRoot(string[] args)
    {
        if (args.Length > 0)
        {
            var candidate = Path.GetFullPath(args[0]);
            if (!File.Exists(Path.Combine(candidate, GitBinModulesFileName)))
            {
                throw new DirectoryNotFoundException(
                    $"'{candidate}' does not contain {GitBinModulesFileName}.");
            }

            return candidate;
        }

        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, GitBinModulesFileName)))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not find {GitBinModulesFileName} in the current directory or any parent directory.");
    }

    /// <summary>
    /// Downloads the SourceForge RSS feed, retrying transient failures.
    /// </summary>
    /// <returns>The raw RSS XML.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the feed cannot be downloaded after three attempts.
    /// </exception>
    private static async Task<string> DownloadFeedAsync()
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

        Exception? lastError = null;

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                return await httpClient.GetStringAsync(FeedUrl);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                lastError = ex;

                if (attempt < 3)
                {
                    Console.WriteLine($"Feed request failed (attempt {attempt}/3): {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        throw new InvalidOperationException($"Could not download {FeedUrl}.", lastError);
    }

    /// <summary>
    /// Finds the newest complete release in the RSS feed.
    /// </summary>
    /// <param name="feedXml">The raw SourceForge RSS XML.</param>
    /// <returns>
    /// The newest release that provides all three archives, or <see langword="null"/>
    /// when the feed does not contain one.
    /// </returns>
    private static ReleaseInfo? FindLatestRelease(string feedXml)
    {
        var document = XDocument.Parse(feedXml);
        var releases = new Dictionary<Version, ReleaseFiles>();

        foreach (var item in document.Descendants("item"))
        {
            var fileName = item.Element("title")?.Value.Trim().TrimStart('/');
            var url = item.Element("link")?.Value.Trim();

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(url))
            {
                continue;
            }

            if (TryGetVersion(Windows32Pattern, fileName) is { } windows32Version)
            {
                GetOrAdd(releases, windows32Version).Windows32Url = url;
            }

            if (TryGetVersion(Windows64Pattern, fileName) is { } windows64Version)
            {
                GetOrAdd(releases, windows64Version).Windows64Url = url;
            }

            if (TryGetVersion(UnixPattern, fileName) is { } unixVersion)
            {
                GetOrAdd(releases, unixVersion).UnixUrl = url;
            }
        }

        var latest = releases
            .Where(pair => pair.Value.IsComplete)
            .OrderByDescending(pair => pair.Key)
            .FirstOrDefault();

        if (latest.Key is null)
        {
            return null;
        }

        return new ReleaseInfo(
            latest.Key,
            latest.Value.Windows32Url!,
            latest.Value.Windows64Url!,
            latest.Value.UnixUrl!);
    }

    /// <summary>
    /// Extracts the version from a file name using the supplied pattern.
    /// </summary>
    /// <param name="pattern">The file name pattern containing a <c>version</c> group.</param>
    /// <param name="fileName">The file name to match.</param>
    /// <returns>The parsed version, or <see langword="null"/> when the name does not match.</returns>
    private static Version? TryGetVersion(Regex pattern, string fileName)
    {
        var match = pattern.Match(fileName);
        return match.Success && Version.TryParse(match.Groups["version"].Value, out var version)
            ? version
            : null;
    }

    /// <summary>
    /// Gets an existing entry from the dictionary or adds a new one.
    /// </summary>
    private static ReleaseFiles GetOrAdd(Dictionary<Version, ReleaseFiles> releases, Version version)
    {
        if (!releases.TryGetValue(version, out var files))
        {
            files = new ReleaseFiles();
            releases[version] = files;
        }

        return files;
    }

    /// <summary>
    /// Writes a value to the GitHub Actions step output file when running in Actions.
    /// </summary>
    private static void WriteGitHubOutput(string name, string value)
    {
        var outputPath = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return;
        }

        File.AppendAllText(outputPath, $"{name}={value}{Environment.NewLine}");
    }

    /// <summary>
    /// Writes a human-readable summary to the GitHub Actions job summary when running in Actions.
    /// </summary>
    private static void WriteGitHubSummary(ReleaseInfo latestRelease)
    {
        var summaryPath = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY");
        if (string.IsNullOrWhiteSpace(summaryPath))
        {
            return;
        }

        var summary = $"""
            ## ExifTool version check

            Latest version on SourceForge: **{latestRelease.Version}**.

            `.gitbinmodules` now points to ExifTool {latestRelease.Version}.

            """;

        File.AppendAllText(summaryPath, summary);
    }

    /// <summary>
    /// Download links discovered for a single release in the SourceForge feed.
    /// </summary>
    private sealed class ReleaseFiles
    {
        public string? Windows32Url { get; set; }

        public string? Windows64Url { get; set; }

        public string? UnixUrl { get; set; }

        public bool IsComplete =>
            !string.IsNullOrEmpty(Windows32Url) &&
            !string.IsNullOrEmpty(Windows64Url) &&
            !string.IsNullOrEmpty(UnixUrl);
    }

    /// <summary>
    /// A complete release with download links for every bundled platform.
    /// </summary>
    private sealed record ReleaseInfo(
        Version Version,
        string Windows32Url,
        string Windows64Url,
        string UnixUrl);
}
