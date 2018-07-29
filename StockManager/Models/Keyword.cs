using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models {
    /// <summary>
    /// Являет собой уникальное ключевове слово.
    /// </summary>
    public class Keyword : Creatable {
        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ObservableCollection<IconKeyword> IconKeywords { get; set; }
            = new ObservableCollection<IconKeyword>();

        public virtual ObservableCollection<Composition> ThemeCompositions { get; set; }
            = new ObservableCollection<Composition>();
    }
}
