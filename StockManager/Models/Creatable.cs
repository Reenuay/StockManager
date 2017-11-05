using System;
using System.ComponentModel.DataAnnotations;
using EntityFramework.Triggers;

namespace StockManager.Models
{
    /// <summary>
    /// Сущность, имеющая дату создания.
    /// </summary>
    abstract class Creatable : Identity
    {
        [Required]
        public DateTime DateCreated { get; private set; }

        static Creatable()
        {
            Triggers<Creatable>.Inserting += entry =>
            {
                entry.Entity.DateCreated = DateTime.Now;
            };
        }
    }
}
