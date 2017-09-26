using System;
using System.Collections.Generic;
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
        public string Name { get; private set; }

        public virtual List<Icon> Icons { get; private set; } = new List<Icon>();

        protected Keyword() { }

        public Keyword(string name, params Icon[] icons)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name == "")
                throw new ArgumentException($"Argument {nameof(name)} can not be empty string.");

            Name = name.ToLower();

            if (icons != null)
                Icons.AddRange(icons);
        }
    }
}
