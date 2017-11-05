using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой тематику наборов иконок.
    /// </summary>
    class Theme : Creatable
    {
        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ObservableCollection<Keyword> Keywords { get; set; }
            = new ObservableCollection<Keyword>();
    }
}
