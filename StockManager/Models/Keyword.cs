using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    public class Keyword : Base
    {
        [Required, Index(IsUnique = true)]
        public string Name { get; private set; }

        public virtual List<Icon> Icons { get; private set; } = new List<Icon>();
    }
}
