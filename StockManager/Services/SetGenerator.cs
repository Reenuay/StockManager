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

        public static event EventHandler SettingsRecalculationCompleted;
        public static event EventHandler GenerationStarted;
        public static event EventHandler GenerationCompleted;
        public static event EventHandler LogChanged;

        public static Theme Theme { get; private set; }
        public static Template Template { get; private set; }
        public static Background Background { get; private set; }
        public static int MaxCombinations { get; private set; }
        public static int Percentage { get; private set; }
        public static BigInteger CombinationsCount { get; private set; }
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

            if (theme.Keywords.Count == 0)
                throw new ArgumentException("Theme is empty.", nameof(theme));

            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (template.Cells.Count == 0)
                throw new ArgumentException("Template is empty.", nameof(template));

            if (percentage < 0)
                throw new ArgumentException("Positive value expected.", nameof(percentage));

            if (percentage > 100)
                throw new ArgumentException("Value less than 100 expected", nameof(percentage));

            if (background != null && background.IsDeleted)
                throw new ArgumentException("Background is deleted", nameof(background));

            Theme = App.GetRepository<Theme>(context).Find(t => t.Id == theme.Id);
            Template = App.GetRepository<Template>(context).Find(t => t.Id == template.Id);
            Background = App.GetRepository<Background>(context).Find(b => b.Id == background.Id);
            MaxCombinations = maxCombinations;
            Percentage = percentage;

            ActionLog = "";
            Counter = 0;

            recalculator.RunWorkerAsync();
        }

        #region Calculation

        private static void FindMatchingIcons()
        {
            var keywordIds = Theme.Keywords.Select(k => k.Id);

            MatchingIcons = App.GetRepository<Icon>(context).Select(
                i => !i.IsDeleted
                    && (
                        i.Keywords.Select(k => k.Id)
                            .Intersect(keywordIds)
                            .Count()
                            * 100.0
                            / i.Keywords.Count
                    )
                    >= Percentage
            );
        }

        private static void CalculateCombinations()
        {
            int iconsCount = MatchingIcons.Count,
                cellsCount = Template.Cells.Count;


            if (iconsCount == 0)
            {
                WriteToLog("None icons match given relevance percentage");
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

            if (cellsCount >= iconsCount / 2)
            {
                CombinationsCount = ProductOfRange(iconsCount, cellsCount)
                    / ProductOfRange(iconsCount - cellsCount, 1);
            }
            else
            {
                CombinationsCount = ProductOfRange(iconsCount, iconsCount - cellsCount)
                    / ProductOfRange(cellsCount, 1);
            }

            // Достаём айдишники для запроса в базу
            var matchingIconsIds = MatchingIcons.Select(i => i.Id);

            ExistingCombinationsCount = App.GetRepository<Set>(context).Select(
                s => s.Icons.Any()
                    && s.Compositions.Any(c => c.WasUsed)
                    && !s.Icons.Select(i => i.Id)
                        .Except(matchingIconsIds)
                        .Any()
            )
            .Count;
        }

        private static BigInteger ProductOfRange(int upper, int lower)
        {
            if (upper < 0 || lower <= 0 || upper < lower)
                return 0;

            if (upper == 0)
                return 1;

            BigInteger product = 1;

            for (var i = upper; i > lower; i--)
            {
                product *= i;
            }

            return product;
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
                Directory.CreateDirectory(setsPath);
            }

            // Достаём айдишники для запроса в базу
            var matchingIconsIds = MatchingIcons.Select(i => i.Id);

            var sets = App.GetRepository<Set>(context).Select(
                s => s.Icons.Any()
                    && !s.Icons.Select(i => i.Id)
                        .Except(matchingIconsIds)
                        .Any()
            );

            var unusedSets = new ObservableCollection<Set>(
                sets.Where(s => !s.Compositions.Any(c => c.WasUsed))
            );

            var n = MatchingIcons.Count;
            var k = Template.Cells.Count;
            var positions = new int[k];
            var index = k - 1;
            var stepSize = MaxCombinations == 0
                ? 1
                : CombinationsCount / MaxCombinations;

            foreach (var i in Enumerable.Range(0, k))
            {
                positions[i] = i;
            }

            if (sets.Count < CombinationsCount)
            {
                var setRepo = App.GetRepository<Set>(context);
                var stop = false;

                setRepo.ExecuteTransaction(() =>
                {
                    while (true)
                    {
                        var newCombination = new ObservableCollection<Icon>();

                        for (var i = 0; i < k; i++)
                        {
                            newCombination.Add(MatchingIcons[positions[i]]);
                        }

                        var snapshot = HashGenerator.TextToMD5(
                            (from i in newCombination select i.CheckSum)
                                .OrderBy(s => s)
                                .Aggregate((a, b) => a + b)
                        );

                        if (sets.All(s => s.Snapshot != snapshot))
                        {
                            var set = new Set
                            {
                                Snapshot = snapshot,
                                Icons = newCombination
                            };

                            setRepo.Insert(set);
                            unusedSets.Add(set);
                        }

                        for (var step = 0; step < stepSize; step++)
                        {
                            for (var i = k - 1; i >= 0; i--)
                            {
                                positions[i]++;

                                if (i < k - 1)
                                {
                                    for (var j = i + 1; j < k; j++)
                                    {
                                        positions[j] = positions[i] + j - i;
                                    }
                                }

                                if (positions[i] > i + n - k)
                                {
                                    if (i == 0)
                                    {
                                        stop = true;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (stop)
                            break;

                        if (stopOperation)
                        {
                            stopOperation = false;
                            break;
                        }
                    }
                });
            }

            WriteToLog("Starting set generation...");
            var compositionRepo = App.GetRepository<Composition>(context);

            foreach (var set in unusedSets)
            {
                if (stopOperation)
                {
                    stopOperation = false;
                    break;
                }

                var composition = compositionRepo.Find(
                    c => c.SetId == set.Id && !c.WasUsed
                );

                if (composition == null)
                {
                    composition = new Composition
                    {
                        Keywords = new ObservableCollection<Keyword>(
                            set.Icons.SelectMany(i => i.Keywords).Distinct()
                        ),
                        Theme = Theme,
                        Set = set,
                        Background = Background,
                        Mappings = new ObservableCollection<Mapping>(
                            Template.Cells.Join(
                                set.Icons,
                                c => Template.Cells.IndexOf(c),
                                i => set.Icons.IndexOf(i),
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

                    compositionRepo.Insert(composition);
                }

                var fileName = "";
                try
                {
                    var setStart = DateTime.Now;

                    WriteToLog("Trying to generate a new set...");
                    WriteToLog($"Started at {setStart}");

                    fileName = IllustratorCaller.CreateComposition(
                        composition,
                        composition.Theme.Name,
                        StartTime.Value.ToString("dd MMMM, yyyy (HH часов mm минут ss секунд)")
                    );

                    WriteToLog($"Trying to write meta...");

                    IllustratorCaller.WriteMeta(
                        $"{fileName}.jpg",
                        composition.Name,
                        composition.Keywords
                            .Select(keyword => keyword.Name)
                    );

                    WriteToLog("Updating database...");

                    composition.WasUsed = true;
                    compositionRepo.Update(composition);
                    Counter++;

                    WriteToLog($"New set created: {fileName}.eps");
                    WriteToLog($"Time elapsed: {(DateTime.Now - setStart).ToString()}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    WriteToLog("Error creating set:" + ex.Message);

                    try
                    {
                        if (File.Exists($"{fileName}.eps"))
                            File.Delete($"{fileName}.eps");

                        if (File.Exists($"{fileName}.jpg"))
                            File.Delete($"{fileName}.jpg");
                    }
                    catch(Exception inEx)
                    {
                        logger.Error(inEx);
                    }
                }

                if (stopOperation)
                {
                    stopOperation = false;
                    break;
                }
            }

            WriteToLog("Finished set generation.");
        }

        private static void WriteToLog(string message)
        {
            ActionLog += "\n" + message;
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

            if (CombinationsCount - ExistingCombinationsCount == 0)
            {
                WriteToLog("No combinations available. Canceling...");
                OnGenerationCompleted(sender, e);
            }

            if (Settings.Default.NameTemplates.Count == 0)
            {
                WriteToLog("No name templates provided. Canceling...");
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
