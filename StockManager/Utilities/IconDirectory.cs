using System.IO;
using System.Linq;
using StockManager.Properties;

namespace StockManager.Utilities
{
    static class IconDirectory
    {
        public static string[] AllowedExtensions
        {
            get
            {
                return Settings.Default.AllowedFileExtensions
                    .Cast<string>()
                    .ToArray();
            }
        }

        public static void Create()
        {
            Directory.CreateDirectory(Settings.Default.IconDirectoryName);
        }

        public static FileSystemWatcher CreateWatcher()
        {
            return new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite
                | NotifyFilters.FileName,

                Path = GetFullPath(),
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
        }

        public static bool Exists()
        {
            return Directory.Exists(Settings.Default.IconDirectoryName);
        }

        public static bool ExtensionIsAllowed(string fullPath)
        {
            return Settings
                .Default
                .AllowedFileExtensions
                .Contains(Path.GetExtension(fullPath));
        }

        public static string GetFullPath()
        {
            return Path.GetFullPath(Settings.Default.IconDirectoryName);
        }

        public static string[] GetIcons()
        {
            return Directory.GetFiles(
                Path.GetFullPath(Settings.Default.IconDirectoryName),
                "*.*",
                SearchOption.AllDirectories
            )
            .Where(path => File.Exists(path) && ExtensionIsAllowed(path))
            .ToArray();
        }
    }
}
