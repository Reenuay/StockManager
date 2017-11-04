using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using ImageMagick;
using PropertyChanged;
using StockManager.Models;
using StockManager.Utilities;

namespace StockManager.ViewModels
{
    /// <summary>
    /// Представляет из себя обёртку для иконки.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    class IconViewModel
    {
        #region Properties

        public Icon Icon { get; private set; }

        public string RelativePath { get; private set; }
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
                RelativePath = icon.FullPath.Remove(
                    0,
                    Environment.CurrentDirectory.Length + 1
                );
                Date = icon.DateCreated.ToString("dd.MM.yyyy");
                Keywords = icon.Keywords;

                if (!icon.IsDeleted)
                {
                    using (var image = new MagickImage(icon.FullPath))
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
                RelativePath = "...";
                Date = "...";
                Size = "...";
                Preview = IconDirectory.PreviewImage;
            }
        }
    }
}
