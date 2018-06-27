using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using ImageMagick;
using StockManager.Models;
using StockManager.Utilities;

namespace StockManager.ViewModels {
    /// <summary>
    /// Представляет из себя обёртку для фона.
    /// </summary>
    class BackgroundViewModel : ViewModelBase {
        #region Properties

        public Background Background { get; private set; }

        public string FullPath { get; private set; }
        public string CheckSum { get; private set; }
        public string Size { get; private set; }
        public string Date { get; private set; }

        public BitmapSource Preview { get; private set; }

        #endregion

        public BackgroundViewModel(Background background = null) {
            if (background != null) {
                Background = background;
                CheckSum = background.CheckSum;
                FullPath = background.FullPath;
                Date = background.DateCreated.ToString("dd.MM.yyyy");

                if (!background.IsDeleted) {
                    var readSettings = new MagickReadSettings {
                        Density = new Density(300, 300)
                    };

                    using (var image = new MagickImage(background.FullPath, readSettings)) {
                        Preview = image.ToBitmapSource();
                    }

                    Size = ((new FileInfo(background.FullPath)).Length / 1024)
                        .ToString() + " KB";
                }
                else {
                    Preview = IconDirectory.PreviewImage;
                    Size = "...";
                }
            }
            else {
                CheckSum = "...";
                FullPath = "...";
                Date = "...";
                Size = "...";
                Preview = IconDirectory.PreviewImage;
            }
        }
    }
}
