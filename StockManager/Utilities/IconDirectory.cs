using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using StockManager.Properties;

namespace StockManager.Utilities
{
    static class IconDirectory
    {
        private static BitmapImage previewImage;

        public static string[] AllowedExtensions
        {
            get
            {
                return Settings.Default.AllowedFileExtensions
                    .Cast<string>()
                    .ToArray();
            }
        }

        public static string FullPath
        {
            get
            {
                return Path.GetFullPath(Settings.Default.IconDirectoryName);
            }
        }

        public static BitmapImage PreviewImage
        {
            get
            {
                if (previewImage == null)
                {
                    previewImage = new BitmapImage(
                        new Uri("/Resources/Preview.png", UriKind.Relative)
                    );
                }

                return previewImage;
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

                Path = FullPath,
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
