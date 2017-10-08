using System.Windows.Media.Imaging;

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

        public BitmapSource Preview { get; set; }
    }
}
