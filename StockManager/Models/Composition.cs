using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Композиция набора иконок по шаблону.
    /// </summary>
    public class Composition : Creatable
    {
        [Required]
        public string Name { get; set; }

        public int SetId { get; set; }
        public int? ThemeId { get; set; }
        public int? ColorId { get; set; }
        public int? BackgroundId { get; set; }

        public virtual Set Set { get; set; }
        public virtual Keyword Theme { get; set; }
        public virtual Color Color { get; set; }
        public virtual Background Background { get; set; }

        public virtual ObservableCollection<Mapping> Mappings { get; set; }
            = new ObservableCollection<Mapping>();

        [NotMapped]
        public Template Template
        {
            get
            {
                if (Mappings.Count == 0)
                    return null;

                return Mappings[0].Cell.Template;
            }
        }
    }
}
