using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using StockManager.Utilities;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой запись файла иконки в базе.
    /// </summary>
    public class Icon : Base
    {
        [Required, Index(IsUnique = true), MinLength(32), MaxLength(32)]
        public string CheckSum { get; private set; }

        [Required]
        public string FullPath { get; private set; }

        public bool IsDeleted { get; set; }

        public virtual List<Keyword> Keywords { get; private set; } = new List<Keyword>();

        [NotMapped]
        public string Name => Path.GetFileNameWithoutExtension(FullPath);

        protected Icon() { }

        public Icon(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"File {path} was not found.", path);

            FullPath = path;
            CheckSum = HashGenerator.FileToMD5(new FileInfo(path));
            IsDeleted = false;
        }
    }
}
