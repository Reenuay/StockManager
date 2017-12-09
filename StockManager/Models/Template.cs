using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Шаблон для генератора наборов иконок.
    /// </summary>
    public class Template : Creatable
    {
        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ObservableCollection<Cell> Cells { get; set; }
            = new ObservableCollection<Cell>();
    }
}
