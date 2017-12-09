using System;
using EntityFramework.Triggers;

namespace StockManager.Models
{
    /// <summary>
    /// Сущность, имеющая дату создания.
    /// </summary>
    public abstract class Creatable : Identity
    {
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
