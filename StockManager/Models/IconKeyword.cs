using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManager.Models {
    public class IconKeyword : Base {
        [Key, ForeignKey(nameof(Icon)), Column(Order = 0)]
        [Index("UniqueIconKeyword", IsUnique = true, Order = 0)]
        [Index("UniqueIconKeywordPriority", IsUnique = true, Order = 0)]
        public int IconId { get; set; }

        [Key, ForeignKey(nameof(Keyword)), Column(Order = 1)]
        [Index("UniqueIconKeyword", IsUnique = true, Order = 1)]
        [Index("UniqueIconKeywordPriority", IsUnique = true, Order = 1)]
        public int KeywordId { get; set; }

        [Index("UniqueIconKeywordPriority", IsUnique = true, Order = 2)]
        public int Priority { get; set; }

        public virtual Icon Icon { get; private set; }
        public virtual Keyword Keyword { get; private set; }
    }
}
