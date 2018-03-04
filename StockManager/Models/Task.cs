using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Представляет собой задание для планировщика.
    /// </summary>
    public class Task : Creatable
    {
        public int ThemeId { get; set; }
        public int TemplateId { get; set; }
        public int? BackgroundId { get; set; }
        public int QueueId { get; set; }

        [Range(0, 100)]
        public int Percentage { get; set; }

        [Range(0, int.MaxValue)]
        public int Maximum { get; set; }

        public virtual Theme Theme { get; set; }
        public virtual Template Template { get; set; }
        public virtual Background Background { get; set; }
        public virtual Queue Queue { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return $"{Theme.Name} - {Template.Name} - {Background?.Name} - {Percentage} % - {Maximum} max";
            }
        }
    }
}
