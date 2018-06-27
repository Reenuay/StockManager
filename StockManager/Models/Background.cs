using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace StockManager.Models {
    /// <summary>
    /// Фон для набора иконок.
    /// </summary>
    public class Background : Changeable {

        [Required, Index(IsUnique = true), MinLength(32), MaxLength(32)]
        public string CheckSum { get; set; }
        [Required]
        public string FullPath { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ObservableCollection<Template> Templates { get; set; }
            = new ObservableCollection<Template>();

        public virtual ObservableCollection<Color> Colors { get; set; }
            = new ObservableCollection<Color>();

        [NotMapped]
        public string Name {
            get {
                return Path.GetFileNameWithoutExtension(FullPath);
            }
        }
    }
}
