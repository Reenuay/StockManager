using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace StockManager.Utilities
{
    /// <summary>
    /// Предоставляет методы для получения чек-сумм файлов
    /// и дальнейшей работы с ними
    /// </summary>
    static class HashGenerator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Возвращает чек-сумму файла.
        /// </summary>
        /// <param name="path">Путь до файла.</param>
        public static string FileToMD5(string path)
        {
            using (var md5Hash = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5Hash.ComputeHash(stream))
                        .Replace("-", "");
                }
            }
        }

        /// <summary>
        /// Пробует получить чек-сумму файла.
        /// </summary>
        /// <param name="fullPath">Путь до файла.</param>
        /// <param name="hash">Строка, в которую будет записан хэш.</param>
        /// <returns>Успешность операции.</returns>
        public static bool TryFileToMD5(string fullPath, out string hash)
        {
            try
            {
                hash = FileToMD5(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            hash = "";
            return false;
        }

        /// <summary>
        /// Проверяет строку на соответствие с хэшом MD5.
        /// </summary>
        /// <param name="hash">Хэш-строка.</param>
        public static bool IsMD5(string hash)
        {
            var md5Checker = new Regex("[0-9A-F]{32}");
            return md5Checker.IsMatch(hash);
        }

        public static string TextToMD5(string text)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(text)))
                    .Replace("-", "");
            }
        }

        public static string TextSequenceToMD5(IEnumerable<string> textSequence)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(
                    Encoding.ASCII.GetBytes(
                        textSequence.OrderBy(s => s).Aggregate((a, b) => a + b)
                    )
                ))
                .Replace("-", "");
            }
        }
    }
}
