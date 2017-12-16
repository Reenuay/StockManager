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
    static class BackgroundDirectory
    {
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
        /// Полный путь до папки фонов
        /// </summary>
        public static string FullPath
        {
            get
            {
                return Path.GetFullPath(Settings.Default.BackgroundDirectoryName);
            }
        }

        /// <summary>
        /// Создаёт папку фонов в качестве подпапки приложения
        /// </summary>
        public static void Create()
        {
            Directory.CreateDirectory(Settings.Default.BackgroundDirectoryName);
        }

        /// <summary>
        /// Создаёт сервис <see cref="FileSystemWatcher"/> для отслеживания
        /// изменений в папке фонов с настроенными параметрами
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
        /// Проверяет, существует ли уже папка фонов
        /// </summary>
        public static bool Exists()
        {
            return Directory.Exists(Settings.Default.BackgroundDirectoryName);
        }

        /// <summary>
        /// Проверяет разрешёно ли в папке фонов расширение файла
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
        public static string[] GetBackgrounds()
        {
            return Directory.GetFiles(
                Path.GetFullPath(Settings.Default.BackgroundDirectoryName),
                "*.*",
                SearchOption.AllDirectories
            )
            .Where(path => File.Exists(path) && ExtensionIsAllowed(path))
            .ToArray();
        }
    }
}
