using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Illustrator;
using StockManager.Models;
using StockManager.Properties;

namespace StockManager.Services
{
    static class IllustratorCaller
    {
        [STAThread]
        public static string CreateComposition(Composition composition, params string[] folders)
        {
            var folderFullPath = Path.Combine(
                new string[] {
                    Environment.CurrentDirectory,
                    Settings.Default.SetsDirectory,
                }
                .Concat(folders)
                .ToArray()
            );

            if (!Directory.Exists(folderFullPath))
            {
                Directory.CreateDirectory(folderFullPath);
            }

            // Открываем иллюстратор
            var ai = new Application();

            // Создаём пустой документ
            var preset = new DocumentPreset
            {
                Width = Settings.Default.DocWidth,
                Height = Settings.Default.DocHeight,
                DocumentUnits = AiRulerUnits.aiUnitsMM,
                DocumentColorSpace = AiDocumentColorSpace.aiDocumentRGBColor
            };

            var targetDoc = ai.Documents.AddDocument(
                "IconSet",
                preset
            );

            // Открываем файл иконки
            var background = ai.Open(
                composition.Background.FullPath,
                AiDocumentColorSpace.aiDocumentRGBColor
            );

            // Разблокируем все слои и элементы
            foreach (dynamic layer in background.Layers)
            {
                layer.Locked = false;
            }

            foreach (dynamic item in background.PageItems)
            {
                item.Locked = false;
            }

            // Группируем все элементы
            ai.ExecuteMenuCommand("selectall");
            ai.ExecuteMenuCommand("group");

            // Переносим группу в основной файл
            var backGroup = (GroupItem)ai.ActiveDocument.Selection[0].duplicate(
                targetDoc,
                AiElementPlacement.aiPlaceAtEnd
            );

            var percentage = backGroup.Width >= backGroup.Height
                ? targetDoc.Width / backGroup.Width
                : targetDoc.Height / backGroup.Height;

            backGroup.Width *= percentage;
            backGroup.Height *= percentage;

            backGroup.Left = 0;
            backGroup.Top = targetDoc.Height;

            // Закрываем файл фона
            background.Close(AiSaveOptions.aiDoNotSaveChanges);
            background = null;

            // Для каждого отображения в композиции
            foreach (var mapping in composition.Mappings)
            {
                // Открываем файл иконки
                var iconDoc = ai.Open(
                    mapping.Icon.FullPath,
                    AiDocumentColorSpace.aiDocumentRGBColor
                );

                ai.ActiveDocument = iconDoc;

                // Разблокируем все слои и элементы
                foreach (dynamic layer in iconDoc.Layers)
                {
                    layer.Locked = false;
                }

                foreach (dynamic item in iconDoc.PageItems)
                {
                    item.Locked = false;
                }

                // Группируем все элементы
                ai.ExecuteMenuCommand("selectall");
                ai.ExecuteMenuCommand("group");

                // Переносим группу в основной файл
                var newGroup = (GroupItem)ai.ActiveDocument.Selection[0].duplicate(
                    targetDoc,
                    AiElementPlacement.aiPlaceAtBeginning
                );

                // Меняем размер копии и позицию
                double x = targetDoc.Width * mapping.Cell.X / 100,
                       y = targetDoc.Height * mapping.Cell.Y / 100,
                       w = targetDoc.Width * mapping.Cell.Width / 100,
                       h = targetDoc.Height * mapping.Cell.Height / 100;

                double gw = newGroup.Width,
                       gh = newGroup.Height;

                percentage = gw >= gh ? w / gw : h / gh;

                newGroup.Width *= percentage;
                newGroup.Height *= percentage;

                newGroup.Left = x + (w - newGroup.Width) / 2;
                newGroup.Top = y + h - (h - newGroup.Height) / 2;

                // Закрываем файл иконки
                iconDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
                iconDoc = null;
            }

            // Сохраненям файл
            var saveIn = Path.Combine(
                folderFullPath,
                composition.Set.Snapshot
            );

            var saveOptions = new EPSSaveOptions
            {
                Preview = AiEPSPreview.aiTransparentColorTIFF,
                Compatibility = AiCompatibility.aiIllustrator10,
                EmbedAllFonts = true,
                CMYKPostScript = true,
                EmbedLinkedFiles = true,
            };

            targetDoc.SaveAs($"{saveIn}.eps", saveOptions);

            // Экспортируем в jpeg
            var exportOptions = new ExportOptionsJPEG
            {
                AntiAliasing = true,
                Optimization = true,
                QualitySetting = 100,
                HorizontalScale = 500,
                VerticalScale = 500,
                ArtBoardClipping = true,
            };

            targetDoc.Export($"{saveIn}.m.jpg", AiExportType.aiJPEG, exportOptions);

            // Закрываем документ
            targetDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
            targetDoc = null;

            return saveIn;
        }

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

                var semicolonSeparatedKeywors
                    = keywords
                        .Take(
                            Settings.Default.JpegMaxKeywords
                                - Settings.Default.RequiredKeywords.Count
                        )
                        .Concat(Settings.Default.RequiredKeywords.Cast<string>())
                        .Aggregate((a, b) => a + ";" + b) + ";";

                var metadata = new BitmapMetadata("jpg");
                metadata.SetQuery(@"/xmp/dc:title", title);
                metadata.SetQuery(@"/app1/ifd/{ushort=40091}", title);
                metadata.SetQuery(@"/app13/irb/8bimiptc/iptc/object name", title);
                metadata.SetQuery(@"/xmp/dc:description", title);
                metadata.SetQuery(@"/app1/ifd/{ushort=37510}", title);
                metadata.SetQuery(@"/app13/irb/8bimiptc/iptc/caption", title);
                metadata.SetQuery(@"/xmp/dc:subject", semicolonSeparatedKeywors);
                metadata.SetQuery(@"/app1/ifd/{ushort=40094}", semicolonSeparatedKeywors);
                metadata.SetQuery(@"/app13/irb/8bimiptc/iptc/keywords", semicolonSeparatedKeywors);

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
