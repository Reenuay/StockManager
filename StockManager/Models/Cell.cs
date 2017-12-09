using System.ComponentModel.DataAnnotations;

namespace StockManager.Models
{
    /// <summary>
    /// Относительная позиция и размер иконки в композиции.
    /// </summary>
    public class Cell : Identity
    {
        [Range(0.0, 100.0)]
        public float X { get; set; }

        [Range(0.0, 100.0)]
        public float Y { get; set; }

        [Range(0.0, 100.0)]
        public float Width { get; set; }

        [Range(0.0, 100.0)]
        public float Height { get; set; }

        public int TemplateId { get; set; }

        public virtual Template Template { get; set; }
    }
}
