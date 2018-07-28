using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using StockManager.Properties;

namespace StockManager.Services {
    static class IllustratorCaller
    {
        [STAThread]
        public static void WriteMeta(string fileNameWithoutExtension, string title, IEnumerable<string> keywords)
        {
            // Записываем ключевые слова
            BitmapDecoder decoder;
            var originalImage = new FileInfo($"{fileNameWithoutExtension}.m.jpg");

            if (originalImage.Exists)
            {
                var inStream = originalImage.Open(FileMode.Open);

                decoder = BitmapDecoder.Create(
                    inStream,
                    BitmapCreateOptions.IgnoreColorProfile,
                    BitmapCacheOption.Default
                );

                var commaSeparatedKeywors
                    = keywords
                        .Except(Settings.Default.RequiredKeywords.Cast<string>())
                        .Take(
                            Settings.Default.JpegMaxKeywords
                                - Settings.Default.RequiredKeywords.Count
                        )
                        .Concat(Settings.Default.RequiredKeywords.Cast<string>())
                        .Aggregate((a, b) => a + ", " + b);

                var metadata = new BitmapMetadata("jpg");
                metadata.SetQuery(@"/xmp/dc:title", title);
                metadata.SetQuery(@"/app1/ifd/{ushort=40091}", title);
                metadata.SetQuery(@"/app13/irb/8bimiptc/iptc/object name", title);
                metadata.SetQuery(@"/xmp/dc:description", title);
                metadata.SetQuery(@"/app1/ifd/{ushort=37510}", title);
                metadata.SetQuery(@"/app13/irb/8bimiptc/iptc/caption", title);
                metadata.SetQuery(@"/xmp/dc:subject", commaSeparatedKeywors);
                metadata.SetQuery(@"/app1/ifd/{ushort=40094}", commaSeparatedKeywors);
                metadata.SetQuery(@"/app13/irb/8bimiptc/iptc/keywords", commaSeparatedKeywors);

                var frame = BitmapFrame.Create(
                    decoder.Frames[0],
                    decoder.Frames[0].Thumbnail,
                    metadata,
                    decoder.Frames[0].ColorContexts
                );

                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(
                    frame,
                    frame.Thumbnail,
                    metadata,
                    frame.ColorContexts
                ));

                using (var jpegStreamOut = File.Open(
                    $"{fileNameWithoutExtension}.jpg",
                    FileMode.Create,
                    FileAccess.Write)
                )
                {
                    encoder.Save(jpegStreamOut);
                }

                inStream.Close();
                originalImage.Delete();
            }
        }
    }
}
