namespace StockManager.Models
{
    /// <summary>
    /// Сущность, имеющая целочисленный уникальный идентификатор.
    /// </summary>
    abstract class Identity : Base
    {
        public int Id { get; private set; }
    }
}
