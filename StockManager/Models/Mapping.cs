namespace StockManager.Models
{
    /// <summary>
    /// Связывает иконку и ячейку в шаблоне.
    /// </summary>
    public class Mapping : Identity
    {
        public int IconId { get; set; }
        public int CellId { get; set; }
        public int CompositionId { get; set; }

        public virtual Icon Icon { get; set; }
        public virtual Cell Cell { get; set; }
        public virtual Composition Composition { get; set; }
    }
}
