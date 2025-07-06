using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpExifTool
{
    public class ExifTool : IDisposable
    {
        private readonly string _exifToolBin;
        private const string Arguments = @"-stay_open 1 -@ - -common_args -charset UTF8 -G1 -args";
        private readonly string _exitCommand
            = string.Join(Environment.NewLine, new string[] { "-stay_open", "0", $"-execute{Environment.NewLine}" });
        private const int Timeout = 30000;    // in milliseconds
        private const int ExitTimeout = 15000;

        private readonly Encoding _utf8NoBom = new UTF8Encoding(false);

        private Process _processExifTool;
        private StreamWriter _writer;
        private StreamReader _reader;

        public ExifTool(string exiftoolPath = "")
        {
            if(string.IsNullOrEmpty(exiftoolPath))
            {
                var currentDir = Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                if (string.IsNullOrWhiteSpace(currentDir))
                    throw new InvalidOperationException();
                        
                _exifToolBin  =
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Path.Combine(currentDir, "ExifTool.Win", "exiftool.exe")
                    : Path.Combine(currentDir, "ExifTool.Unix", "exiftool");
            }
            else
            {
                if(!File.Exists(exiftoolPath))
                    throw new FileNotFoundException("ExifTool not found.", exiftoolPath);
                _exifToolBin = exiftoolPath;
            }

            // Prepare process start
            var psi = new ProcessStartInfo(_exifToolBin, Arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = _utf8NoBom
            };

            try
            {
                _processExifTool = Process.Start(psi);
                if (_processExifTool == null || _processExifTool.HasExited)
                {
                    throw new ApplicationException("Failed to launch ExifTool!");
                }
            }
            catch (System.ComponentModel.Win32Exception err)
            {
                throw new ApplicationException("Failed to load ExifTool. 'ExifTool.exe' should be located in the same directory as the application or on the path.", err);
            }

            // ProcessStartInfo in .NET Framework doesn't have a StandardInputEncoding property (though it does in .NET Core)
            // So, we have to wrap it this way.
            _writer = new StreamWriter(_processExifTool.StandardInput.BaseStream, _utf8NoBom);
            _reader = _processExifTool.StandardOutput;
        }

        /*
        public Task<int> ExecuteAsync(string args)
        {
            return Task.FromResult(Execute(args));
        }
        */

        public Task<int> ExecuteAsync(params string[] args)
        {
            return Task.FromResult(Execute(args));
        }

        public Task<int> ExecuteAsync(IEnumerable<string> args)
        {
            return Task.FromResult(Execute(args));
        }

        /*
        public int Execute(string args)
        {
            var argList = args
                .Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            return Execute(argList);
        }
        */
        
        public int Execute(params string[] args)
        {
            return Execute((IEnumerable<string>) args);
        }

        public int Execute(IEnumerable<string> args)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.AppendLine(arg);
            }

            sb.AppendLine("-execute");

            _writer.Write(sb.ToString());
            _writer.Flush();

            return 0;
        }

        public Task<ICollection<KeyValuePair<string, string>>> ExtractAllMetadataAsync(string filename, params string[] args)
        {
            return Task.FromResult(ExtractAllMetadata(filename, args));
        }

        public ICollection<KeyValuePair<string, string>> ExtractAllMetadata(string filename, params string[] args)
        {
            var commands = new List<string> {};
            if (args != null && args.Length > 0)
            {
                commands.AddRange(args);
            }
            commands.Add(filename);

            Execute(commands);

            var result = new List<KeyValuePair<string, string>>();

            while(true)
            {
                var line = _reader.ReadLine();

                if (line.StartsWith("{ready")) break;
                if (line[0] == '-')
                {
                    int eq = line.IndexOf('=');
                    if (eq > 1)
                    {
                        string key = line.Substring(1, eq - 1);
                        string value = line.Substring(eq + 1).Trim();
                        result.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
            }

            return result;
        }

        public Task<int> RemoveAllMetadataAsync(string filename, bool overwriteOriginal = false)
        {
            return Task.FromResult(RemoveAllMetadata(filename, overwriteOriginal));
        }

        public int RemoveAllMetadata(string filename, bool overwriteOriginal = false)
        {
            WriteTags(
                filename, 
                new Dictionary<string, string> { ["all"] = "" },
                overwriteOriginal);
            
            return 0;
        }

        public Task<int> WriteTagsAsync(string filename, ICollection<KeyValuePair<string, string>> properties, bool overwriteOriginal = false)
        {
            return Task.FromResult(WriteTags(filename, properties, overwriteOriginal));
        }
        public int WriteTags(string filename, ICollection<KeyValuePair<string, string>> properties, bool overwriteOriginal = false)
        {
            var commands = new List<string> { };

            foreach(var  property in properties)
            {
                commands.Add($"-{property.Key}={property.Value}");
            }

            if (overwriteOriginal)
                commands.Add("-overwrite_original");

            commands.Add(filename);

            Execute(commands);

            var line = _reader.ReadLine();
            Debug.WriteLine(line);

            return 0;
        }

        #region IDisposable Support

        private void Dispose(bool disposing)
        {
            if (_processExifTool == null) 
                return;
            
            if (!disposing)
            {
                System.Diagnostics.Debug.Fail("Failed to dispose ExifTool.");
            }

            // If the process is running, shut it down cleanly
            if (!_processExifTool.HasExited)
            {
                _writer.Write(_exitCommand);
                _writer.Flush();

                if (!_processExifTool.WaitForExit(ExitTimeout))
                {
                    _processExifTool.Kill();
                    Debug.Fail("Timed out waiting for exiftool to exit.");
                }
#if EXIF_TRACE
                    else
                    {
                        Debug.WriteLine("ExifTool exited cleanly.");
                    }
#endif
            }

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
            _processExifTool.Dispose();
            _processExifTool = null;
        }

        ~ExifTool()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Attempt to parse a date-time in the format used by ExifTool
        /// </summary>
        /// <param name="s">The string to be parsed</param>
        /// <param name="kind">The <see cref="DateTimeKind"/> to be assigned to the resulting value. It is generally
        /// determined by the definition of the corresponding field.</param>
        /// <param name="date">The resulting parsed date.</param>
        /// <returns>True if successful, else false.</returns>
        /// <remarks>
        /// <para>ExifTool formats dates as follows: "YYYY:MM:DD hh:mm:ss". For example, "2018:06:22 19:32:53".</para>
        /// </remarks>
        public static bool TryParseDate(string s, DateTimeKind kind, out DateTime date)
        {
            date = DateTime.MinValue;
            int year, month, day, hour, minute, second;
            s = s.Trim();
            if (!int.TryParse(s.Substring(0, 4), out year)) return false;
            if (s[4] != ':') return false;
            if (!int.TryParse(s.Substring(5, 2), out month)) return false;
            if (s[7] != ':') return false;
            if (!int.TryParse(s.Substring(8, 2), out day)) return false;
            if (s[10] != ' ') return false;
            if (!int.TryParse(s.Substring(11, 2), out hour)) return false;
            if (s[13] != ':') return false;
            if (!int.TryParse(s.Substring(14, 2), out minute)) return false;
            if (s[16] != ':') return false;
            if (!int.TryParse(s.Substring(17, 2), out second)) return false;

            if (year < 1900 || year > 2200) return false;
            if (month < 1 || month > 12) return false;
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (minute < 0 || minute > 59) return false;
            if (second < 0 || second > 59) return false;

            try
            {
                date = new DateTime(year, month, day, hour, minute, second, 0, kind);
            }
            catch (Exception)
            {
                return false; // Probaby a month with too many days.
            }

            return true;
        }

        #endregion Static Methods
    }
}
