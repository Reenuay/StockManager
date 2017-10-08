using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using StockManager.Properties;

namespace StockManager.Utilities
{
    /// <summary>
    /// Хранит методы и свойства для работы с папкой иконок
    /// </summary>
    static class IconDirectory
    {
        private static BitmapImage previewImage;

        /// <summary>
        /// Расширения файлов допущенных к работе в приложении
        /// </summary>
        public static string[] AllowedExtensions
        {
            get
            {
                return Settings.Default.AllowedFileExtensions
                    .Cast<string>()
                    .ToArray();
            }
        }

        /// <summary>
        /// Полный путь до папки иконок
        /// </summary>
        public static string FullPath
        {
            get
            {
                return Path.GetFullPath(Settings.Default.IconDirectoryName);
            }
        }

        /// <summary>
        /// Возвращает <see cref="BitmapSource"/> для файла
        /// предпросмотра иконок
        /// </summary>
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

        /// <summary>
        /// Создаёт папку иконок в качестве подпапки приложения
        /// </summary>
        public static void Create()
        {
            Directory.CreateDirectory(Settings.Default.IconDirectoryName);
        }

        /// <summary>
        /// Создаёт сервис <see cref="FileSystemWatcher"/> для отслеживания
        /// изменений в папке иконок с настроенными параметрами
        /// </summary>
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

        /// <summary>
        /// Проверяет, существует ли уже папка иконок
        /// </summary>
        public static bool Exists()
        {
            return Directory.Exists(Settings.Default.IconDirectoryName);
        }

        /// <summary>
        /// Проверяет разрешёно ли в папке иконок расширение файла
        /// </summary>
        /// <param name="fullPath">Полный путь до файла с расширением</param>
        public static bool ExtensionIsAllowed(string fullPath)
        {
            return Settings
                .Default
                .AllowedFileExtensions
                .Contains(Path.GetExtension(fullPath));
        }

        /// <summary>
        /// Возвращает все пути до файлов разрешённого типа
        /// </summary>
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
