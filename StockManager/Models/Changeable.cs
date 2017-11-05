using System;
using EntityFramework.Triggers;

namespace StockManager.Models
{
    /// <summary>
    /// Сущность, имеющая дату последнего изменения.
    /// </summary>
    public abstract class Changeable : Creatable
    {
        public DateTime? DateChanged { get; private set; }

        static Changeable()
        {
            Triggers<Changeable>.Updating
                += entry => entry.Entity.DateChanged = DateTime.Now;
        }
    }
}
