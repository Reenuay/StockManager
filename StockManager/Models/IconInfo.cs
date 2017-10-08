using System.Collections.Generic;
using System.Windows.Media.Imaging;
using StockManager.Utilities;

namespace StockManager.Models
{
    /// <summary>
    /// Предоставляет контейнер для группировки свойств файлов иконок из
    /// различных источников данных
    /// </summary>
    class IconInfo
    {
        public string RelativePath { get; set; } = "...";
        public string CheckSum { get; set; } = "...";

        public string Size { get; set; } = "...";
        public string Date { get; set; } = "...";

        public BitmapSource Preview { get; set; } = IconDirectory.PreviewImage;
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();
    }
}
