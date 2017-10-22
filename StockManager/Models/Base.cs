using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework.Triggers;

namespace StockManager.Models
{
    /// <summary>
    /// Предоставляет базовый функционал для всех
    /// сущностей базы данных в приложении.
    /// </summary>
    public abstract class Base
    {
        public int Id { get; private set; }

        [Required]
        public DateTime DateCreated { get; private set; }

        [Required]
        public DateTime DateChanged { get; private set; }

        static Base()
        {
            Triggers<Base>.Inserting += entry =>
            {
                entry.Entity.DateCreated
                  = entry.Entity.DateChanged
                  = DateTime.Now;
            };

            Triggers<Base>.Updating
                += entry => entry.Entity.DateChanged = DateTime.Now;
        }
    }
}
