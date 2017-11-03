using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой уникальное ключевове слово.
    /// </summary>
    public class Keyword : Base
    {
        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ObservableCollection<Icon> Icons { get; set; }
            = new ObservableCollection<Icon>();
    }
}
