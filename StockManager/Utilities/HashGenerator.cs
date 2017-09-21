using System;
using System.IO;
using System.Security.Cryptography;

namespace StockManager.Utilities
{
    static class HashGenerator
    {
        public static string FileToMD5(FileInfo file)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                using (FileStream stream = file.OpenRead())
                {
                    return BitConverter.ToString(md5Hash.ComputeHash(stream)).Replace("-", "");
                }
            }
        }
    }
}
