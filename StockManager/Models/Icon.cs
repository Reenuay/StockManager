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
        public string CheckSum { get; set; }

        [Required]
        public string FullPath { get; set; }

        public bool IsDeleted { get; set; }

        public virtual List<Keyword> Keywords { get; set; }
            = new List<Keyword>();

        [NotMapped]
        public string Name
        {
            get
            {
                return Path.GetFileName(FullPath);
            }
        }
    }
}
