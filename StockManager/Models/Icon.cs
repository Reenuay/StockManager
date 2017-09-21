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
        [Required, Index("IconUniqueFile", 1, IsUnique = true)]
        public string FullPath { get; private set; }

        [Required, Index("IconUniqueFile", 2, IsUnique = true), MinLength(32), MaxLength(32)]
        public string CheckSum { get; private set; }

        public bool IsDeleted { get; set; }

        public virtual List<Keyword> Keywords { get; private set; } = new List<Keyword>();

        protected Icon() { }

        public Icon(FileInfo file)
        {
            FullPath = file.FullName;
            CheckSum = HashGenerator.FileToMD5(file);
            IsDeleted = false;
        }
    }
}
