using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой запись файла иконки в базе.
    /// </summary>
    public class Icon : Changeable
    {
        [Required]
        public string Name { get; set; }

        [Required, Index(IsUnique = true), MinLength(32), MaxLength(32)]
        public string CheckSum { get; set; }

        [Required]
        public string FullPath { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ObservableCollection<Keyword> Keywords { get; set; }
            = new ObservableCollection<Keyword>();

        public virtual ObservableCollection<Set> Sets { get; set; }
            = new ObservableCollection<Set>();
    }
}
