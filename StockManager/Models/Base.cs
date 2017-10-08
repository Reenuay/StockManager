using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models
{
    /// <summary>
    /// Предоставляет базовый функционал для всех
    /// классов-моделей базы данных в приложении.
    /// </summary>
    public abstract class Base
    {
        public int Id { get; private set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; private set; }
    }
}
