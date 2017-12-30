using System;
using System.IO;
using Illustrator;
using StockManager.Models;
using StockManager.Properties;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

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

            // Разблокируем все слои
            foreach (dynamic layer in background.Layers)
            {
                if (layer.Locked)
                {
                    layer.Locked = false;
                }
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

                // Разблокируем все слои
                foreach (dynamic layer in iconDoc.Layers)
                {
                    if (layer.Locked)
                    {
                        layer.Locked = false;
                    }
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
                composition.Mappings
                    .Select(m => m.IconId.ToString())
                    .Aggregate((a, b) => a + "," + b)
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
                QualitySetting = 12,
                HorizontalScale = 500,
                VerticalScale = 500,
                ArtBoardClipping = true,
            };

            targetDoc.Export($"{saveIn}.jpg", AiExportType.aiJPEG, exportOptions);

            // Закрываем документ
            targetDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
            targetDoc = null;

            return saveIn;
        }

        public static void WriteMeta(string file, string title, IEnumerable<string> keywords)
        {
            // Записываем ключевые слова
            JpegBitmapDecoder decoder;
            var originalImage = new FileInfo(file);

            if (originalImage.Exists)
            {
                using (var inStream = File.Open(file, FileMode.Open))
                {
                    decoder = new JpegBitmapDecoder(
                        inStream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad
                    );
                }

                var frame = decoder.Frames[0];
                var meta = (BitmapMetadata)frame.Metadata.Clone();
                meta.Keywords = new ReadOnlyCollection<string>(
                    keywords.Concat(
                        Settings.Default.DefaultKeywords.Cast<string>()
                    )
                    .ToList()
                );
                meta.Title = title;

                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(
                    frame,
                    frame.Thumbnail,
                    meta,
                    frame.ColorContexts
                ));

                originalImage.Delete();

                using (var jpegStreamOut = File.Open(file, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    encoder.Save(jpegStreamOut);
                }
            }
        }
    }
}
