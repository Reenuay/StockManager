using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using NLog;
using StockManager.Models;
using StockManager.Properties;
using StockManager.Utilities;

namespace StockManager.Services
{
    static class SetGenerator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static BackgroundWorker generator;
        private static BackgroundWorker recalculator;
        private static bool stopOperation;
        private static Context context = new Context();
        private static CombinationEnumerator cEnumerator;

        public static event EventHandler SettingsRecalculationCompleted;
        public static event EventHandler GenerationStarted;
        public static event EventHandler GenerationCompleted;
        public static event EventHandler LogChanged;

        public static Theme Theme { get; private set; }
        public static Template Template { get; private set; }
        public static Background Background { get; private set; }
        public static int MaxCombinations { get; private set; }
        public static int Percentage { get; private set; }
        public static BigInteger ExistingCombinationsCount { get; private set; }
        public static ObservableCollection<Icon> MatchingIcons { get; private set; }
        public static DateTime? StartTime { get; private set; }
        public static DateTime? FinishTime { get; private set; }
        public static string ActionLog { get; private set; } = "";
        public static int Counter { get; private set; }

        public static bool IsWorking
        {
            get
            {
                return generator.IsBusy || recalculator.IsBusy;
            }
        }

        public static void SetSettingsAndStart(Theme theme, Template template, Background background, int percentage = 50, int maxCombinations = 0)
        {
            if (recalculator.IsBusy || generator.IsBusy)
                return;

            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            //if (theme.Keywords.Count == 0)
            //    throw new ArgumentException("Theme is empty.", nameof(theme));

            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (template.Cells.Count == 0)
                throw new ArgumentException("Template is empty.", nameof(template));

            if (percentage < 0)
                throw new ArgumentException("Positive value expected.", nameof(percentage));

            if (percentage > 100)
                throw new ArgumentException("Value less than or equal to 100 expected.", nameof(percentage));

            if (background != null && background.IsDeleted)
                throw new ArgumentException("Background is deleted.", nameof(background));

            //Theme = context.Themes.Find(theme.Id);
            Template = context.Templates.Find(template.Id);
            Background = context.Backgrounds.Find(background.Id);

            MaxCombinations = maxCombinations;
            Percentage = percentage;

            Counter = 0;

            recalculator.RunWorkerAsync();
        }

        #region Calculation

        private static void FindMatchingIcons()
        {
            /*
            MatchingIcons = new ObservableCollection<Icon>(
                context
                .Icons
                .Include("Keywords")
                .Where(
                    i => !i.IsDeleted && i.Keywords.Any()
                )
                .ToList()
                .Where(i => i.Keywords
                    .Intersect(Theme.Keywords)
                    .Any()
                )
            );
            */
        }

        private static void CalculateCombinations()
        {
            int iconsCount = MatchingIcons.Count,
                cellsCount = Template.Cells.Count;

            if (iconsCount == 0)
            {
                WriteToLog("None icons match given relevance percentage. Canceling...");
                OnGenerationCompleted(null, null);
                return;
            }

            if (cellsCount == 0)
            {
                WriteToLog("Template must have at least 1 cell. Canceling...");
                OnGenerationCompleted(null, null);
                return;
            }

            if (iconsCount < cellsCount)
            {
                WriteToLog(
                    $"Not enough icons - {iconsCount}"
                        + $" for template with cell count - {cellsCount}"
                        + ". Canceling..."
                );
                OnGenerationCompleted(null, null);
                return;
            }

            cEnumerator = new CombinationEnumerator();
            cEnumerator.ChangeValues(iconsCount, cellsCount);

            // Достаём айдишники для запроса в базу
            var matchingIconsIds = MatchingIcons.Select(i => i.Id);

            ExistingCombinationsCount = context.Sets.LongCount(
                s => s.Icons.Count == cellsCount
                    && s.Compositions.Any()
                    && !s.Icons.Select(i => i.Id)
                        .Except(matchingIconsIds)
                        .Any()
            );
        }

        #endregion

        private static void Generate()
        {
            var setsPath = Path.Combine(
                Environment.CurrentDirectory,
                Settings.Default.SetsDirectory
            );

            if (!Directory.Exists(setsPath))
            {
                WriteToLog($"Created sets directory on {setsPath}.");
                Directory.CreateDirectory(setsPath);
            }

            WriteToLog($"Starting set generation on {cEnumerator.N} icons"
                + $" and {cEnumerator.K} cells..."
            );

            var positions = new int[cEnumerator.K];
            var random = false;
            var dice = new Random();

            if (MaxCombinations > 0) {
                random = true;
            }

            while (true)
            {
                if (random)
                    positions = cEnumerator.Random;
                else
                    positions = cEnumerator.Next;

                // Получаем новую комбинацию.
                var newCombination = new ObservableCollection<Icon>();
                for (var i = 0; i < cEnumerator.K; i++)
                {
                    newCombination.Add(MatchingIcons[positions[i]]);
                }

                // Вычисляем её снимок.
                var snapshot = HashGenerator.TextSequenceToMD5(
                    from i in newCombination select i.CheckSum
                );

                // Ищем в базе сет с таким снимком.
                var set = context.Sets.SingleOrDefault(s => s.Snapshot == snapshot);

                // Если сет не найден - создаём.
                if (set == null)
                {
                    set = new Set
                    {
                        Snapshot = snapshot,
                        Icons = newCombination
                    };

                    context.Sets.Add(set);
                    context.SaveChanges();
                }

                // Если композиций с данным сетом нет в базе.
                if (!context.Compositions.Any(c => c.SetId == set.Id))
                {
                    // Список приоритетов иконок в сортировке
                    var randomOrders = set.Icons
                        .Select(p => dice.Next())
                        .ToArray();

                    // Список, случайно подорбранных, позиций иконок.
                    var iconPositions = Enumerable
                        .Range(0, set.Icons.Count)
                        .OrderBy(i => randomOrders[i])
                        .ToArray();

                    // Создаём новую композицию.
                    var composition = new Composition
                    {
                        //Theme = Theme,
                        Set = set,
                        Background = Background,
                        Mappings = new ObservableCollection<Mapping>(
                            Template.Cells.Join(
                                set.Icons,
                                c => Template.Cells.IndexOf(c),
                                i => iconPositions[set.Icons.IndexOf(i)],
                                (c, i) => new Mapping
                                {
                                    Cell = c,
                                    Icon = i
                                }
                            )
                        ),
                    };

                    composition.Name
                        = NameGenerator.GenerateName(composition);

                    // Пытаемся создать новый сет, записать мету и сохранить композицию в базу.
                    var fileName = "";
                    try
                    {
                        // Дата и время начала генрации сета.
                        var setStart = DateTime.Now;

                        WriteToLog("Trying to generate a new set...");
                        WriteToLog($"Started at {setStart}");

                        // Создаём eps и jpg файлы.
                        fileName = IllustratorCaller.CreateComposition(
                            composition,
                            //composition.Theme.Name,
                            StartTime.Value.ToString("dd MMMM, yyyy (HH часов mm минут ss секунд)")
                        );

                        WriteToLog($"Trying to write meta...");

                        // Записываем метаданные в jpg.
                        IllustratorCaller.WriteMeta(
                            fileName,
                            composition.Name,
                            composition.Set.Icons
                                .SelectMany(i => i.Keywords)
                                .Distinct()
                                .Select(keyword => keyword.Name)
                        );

                        WriteToLog("Updating database...");

                        // Записываем композицию в базу.
                        context.Compositions.Add(composition);
                        context.SaveChanges();

                        Counter++;

                        WriteToLog($"New set created: {fileName}.eps");
                        WriteToLog($"Time elapsed: {(DateTime.Now - setStart).ToString()}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        WriteToLog("Error creating set:" + ex.Message);

                        // При ошибках попытаемся удалить файлы.
                        try
                        {
                            if (File.Exists($"{fileName}.eps"))
                                File.Delete($"{fileName}.eps");

                            if (File.Exists($"{fileName}.m.jpg"))
                                File.Delete($"{fileName}.m.jpg");

                            if (File.Exists($"{fileName}.jpg"))
                                File.Delete($"{fileName}.jpg");
                        }
                        catch (Exception inEx)
                        {
                            logger.Error(inEx);
                        }
                    }
                }

                // Юзер запросил окончание
                if (stopOperation)
                {
                    stopOperation = false;
                    break;
                }

                if (MaxCombinations != 0 && Counter >= MaxCombinations)
                    break;
            }

            WriteToLog("Finished set generation.");
        }

        private static void WriteToLog(string message)
        {
            ActionLog = message;
            LogChanged?.Invoke(null, new EventArgs());
        }

        public static void StopOperations()
        {
            WriteToLog("Requested operations stop.");
            stopOperation = true;
        }

        #region OnEvent

        private static void OnSettingsRecalculationStarted(object sender, DoWorkEventArgs e)
        {
            StartTime = DateTime.Now;
            FinishTime = null;
            FireGenerationStarted(sender);
            FindMatchingIcons();
            CalculateCombinations();
        }

        private static void OnSettingsRecalculationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FireSettingsRecalculationCompleted(sender);

            if (Settings.Default.NameTemplates.Count == 0)
            {
                WriteToLog("No name templates provided. Canceling...");
                OnGenerationCompleted(sender, e);
                return;
            }

            if (Template.Cells.Count > MatchingIcons.Count)
            {
                return;
            }

            generator.RunWorkerAsync();
        }

        private static void OnGenerationStarted(object sender, DoWorkEventArgs e)
        {
            Generate();
        }

        private static void OnGenerationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FinishTime = DateTime.Now;
            FireGenerationCompleted(sender);
        }

        #endregion

        #region Fire

        private static void FireSettingsRecalculationCompleted(object sender)
        {
            WriteToLog("Recalculation finished...");
            SettingsRecalculationCompleted?.Invoke(sender, new EventArgs());
        }

        private static void FireGenerationStarted(object sender)
        {
            WriteToLog($"Set generation started at {StartTime}");
            GenerationStarted?.Invoke(sender, new EventArgs());
        }

        private static void FireGenerationCompleted(object sender)
        {
            WriteToLog($"Set generation stopped at {FinishTime}");
            WriteToLog($"Total set generated: {Counter}");
            WriteToLog($"Total time elapsed: {(FinishTime - StartTime).ToString()}");
            GenerationCompleted?.Invoke(sender, new EventArgs());
        }

        #endregion

        static SetGenerator()
        {
            recalculator = new BackgroundWorker();

            recalculator.DoWork += OnSettingsRecalculationStarted;
            recalculator.RunWorkerCompleted += OnSettingsRecalculationCompleted;

            generator = new BackgroundWorker();

            generator.DoWork += OnGenerationStarted;
            generator.RunWorkerCompleted += OnGenerationCompleted;
        }
    }
}
