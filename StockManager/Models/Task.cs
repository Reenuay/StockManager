using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace StockManager.Models
{
    /// <summary>
    /// Представляет собой задание для планировщика.
    /// </summary>
    public class Task : Creatable
    {
        public int ThemeId { get; set; }
        public int TemplateId { get; set; }
        public int BackgroundId { get; set; }

        [Range(0, 100)]
        public int Percentage { get; set; }

        [Range(0, int.MaxValue)]
        public int Maximum { get; set; }

        public Theme Theme { get; set; }
        public Template Template { get; set; }
        public Background Background { get; set; }

        public ObservableCollection<Queue> Queues { get; private set; }
            = new ObservableCollection<Queue>();
    }
}
