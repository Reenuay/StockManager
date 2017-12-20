using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Набор иконок.
    /// </summary>
    public class Set : Creatable
    {
        [Required, Index(IsUnique = true), MinLength(32), MaxLength(32)]
        public string Snapshot { get; set; }

        public virtual ObservableCollection<Icon> Icons { get; set; }
            = new ObservableCollection<Icon>();

        public virtual ObservableCollection<Composition> Compositions { get; set; }
            = new ObservableCollection<Composition>();
    }
}
