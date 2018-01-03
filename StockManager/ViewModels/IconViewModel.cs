using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using ImageMagick;
using StockManager.Models;
using StockManager.Utilities;

namespace StockManager.ViewModels
{
    /// <summary>
    /// Представляет из себя обёртку для иконки.
    /// </summary>
    class IconViewModel : ViewModelBase
    {
        #region Properties

        public Icon Icon { get; private set; }

        public string FullPath { get; private set; }
        public string CheckSum { get; private set; }
        public string Size { get; private set; }
        public string Date { get; private set; }

        public BitmapSource Preview { get; private set; }
        public ObservableCollection<Keyword> Keywords { get; private set; }

        #endregion

        public IconViewModel(Icon icon = null)
        {
            if (icon != null)
            {
                Icon = icon;
                CheckSum = icon.CheckSum;
                FullPath = icon.FullPath;
                Date = icon.DateCreated.ToString("dd.MM.yyyy");
                Keywords = icon.Keywords;

                if (!icon.IsDeleted)
                {
                    var readSettings = new MagickReadSettings
                    {
                        Density = new Density(300, 300)
                    };

                    using (var image = new MagickImage(icon.FullPath, readSettings))
                    {
                        Preview = image.ToBitmapSource();
                    }

                    Size = ((new FileInfo(icon.FullPath)).Length / 1024)
                        .ToString() + " KB";
                }
                else
                {
                    Preview = IconDirectory.PreviewImage;
                    Size = "...";
                }
            }
            else
            {
                CheckSum = "...";
                FullPath = "...";
                Date = "...";
                Size = "...";
                Preview = IconDirectory.PreviewImage;
            }
        }
    }
}
