using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;

namespace StockManager.Utilities
{
    static class HashGenerator
    {
        public static string FileToMD5(string path)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5Hash.ComputeHash(stream))
                        .Replace("-", "");
                }
            }
        }

        public static bool IsMD5(string hash)
        {
            Regex md5Checker = new Regex("[0-9A-F]{32}");
            return md5Checker.IsMatch(hash);
        }
    }
}
