using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using NLog;
using StockManager.Models;
using StockManager.Utilities;

namespace StockManager.Services
{
    static class SetGenerator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static BackgroundWorker generator;
        private static BackgroundWorker recalculator;
        private static Theme theme;
        private static Template template;
        private static Background background;

        private static ObservableCollection<Icon> matchingIcons;

        public static EventHandler SettingsRecalculationStarted;
        public static EventHandler SettingsRecalculationCompleted;

        public static EventHandler GenerationStarted;
        public static EventHandler GenerationCompleted;

        public static int Percentage { get; private set; } = 50;
        public static BigInteger CombinationsCount { get; private set; } = 0;
        public static BigInteger ExistingCombinationsCount { get; private set; } = 0;

        public static void SetSettings(int percentage, Theme theme, Template template, Background background)
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

            SetGenerator.theme = theme;
            SetGenerator.template = template;
            SetGenerator.background = background;

            Percentage = percentage;

            recalculator.RunWorkerAsync();
        }

        private static void FindMatchingIcons()
        {
            matchingIcons = App.GetRepository<Icon>().Select(
                i => !i.IsDeleted
                    && (
                        i.Keywords.Intersect(theme.Keywords, new IdentityEqualityComparer<Keyword>()).Count()
                            * 100.0
                            / i.Keywords.Count
                    )
                    >= Percentage
            );
        }

        private static void CalculateCombinations()
        {
            uint iconsCount = (uint)matchingIcons.Count,
                 cellsCount = (uint)template.Cells.Count;

            if (cellsCount >= iconsCount / 2)
            {
                CombinationsCount = ProductOfRange(iconsCount, cellsCount)
                    / ProductOfRange(cellsCount - iconsCount, 0);
            }
            else
            {
                CombinationsCount = ProductOfRange(cellsCount, cellsCount - iconsCount)
                    / ProductOfRange(iconsCount, 0);
            }

            ExistingCombinationsCount = App.GetRepository<Set>().Select(
                s => !s.Icons.Except(matchingIcons, new IdentityEqualityComparer<Icon>()).Any() && s.Compositions.Any()
            ).Count;
        }

        private static BigInteger ProductOfRange(uint upper, uint lower)
        {
            if (upper < lower)
                return 0;

            BigInteger product = 1;

            for (var i = upper; i > lower; i--)
            {
                product *= i;
            }

            return product;
        }

        public static void StartSetGeneration()
        {
            if (generator.IsBusy || recalculator.IsBusy)
                return;

            if (CombinationsCount - ExistingCombinationsCount == 0)
                return;

            generator.RunWorkerAsync();
        }

        private static void Generate()
        {
            var sets = App.GetRepository<Set>().Select(
                s => !s.Icons.Except(matchingIcons, new IdentityEqualityComparer<Icon>()).Any()
            );

            var unusedSets = new ObservableCollection<Set>(
                sets.Where(s => !s.Compositions.Any())
            );

            var n = matchingIcons.Count;
            var k = template.Cells.Count;
            var positions = new int[k];
            var index = k - 1;

            foreach (var i in Enumerable.Range(0, k))
            {
                positions[i] = i;
            }

            if (sets.Count < CombinationsCount)
            {
                var setRepo = App.GetRepository<Set>();
                var stop = false;

                setRepo.ExecuteTransaction(() =>
                {
                    while (true)
                    {
                        var newCombination = new ObservableCollection<Icon>();

                        for (var i = 0; i < k; i++)
                        {
                            newCombination.Add(matchingIcons[positions[i]]);
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

                        if (stop)
                            break;
                    }
                });

                var compositionRepo = App.GetRepository<Composition>();

                compositionRepo.ExecuteTransaction(() =>
                {
                    foreach (var set in unusedSets)
                    {
                        var composition = new Composition
                        {
                            Keywords = new ObservableCollection<Keyword>(
                                set.Icons.SelectMany(i => i.Keywords).Distinct()
                            ),
                            Theme = theme,
                            Background = background,
                            Mappings = new ObservableCollection<Mapping>(
                                template.Cells.Join(
                                    set.Icons,
                                    c => template.Cells.IndexOf(c),
                                    i => set.Icons.IndexOf(i),
                                    (c, i) => new Mapping
                                    {
                                        Cell = c,
                                        Icon = i
                                    }
                                )
                            )
                        };

                        compositionRepo.Insert(composition);
                    }
                });
            }
        }

        private static void OnSettingsRecalculationStarted(object sender, DoWorkEventArgs e)
        {
            FireSettingsRecalculationStarted(sender);
            FindMatchingIcons();
            CalculateCombinations();
        }

        private static void OnSettingsRecalculationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FireSettingsRecalculationCompleted(sender);
        }

        private static void OnGenerationStarted(object sender, DoWorkEventArgs e)
        {
            FireGenerationStarted(sender);
            Generate();
        }

        private static void OnGenerationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FireGenerationCompleted(sender);
        }

        private static void FireSettingsRecalculationStarted(object sender)
        {
            SettingsRecalculationStarted?.Invoke(sender, new EventArgs());
        }

        private static void FireSettingsRecalculationCompleted(object sender)
        {
            SettingsRecalculationCompleted?.Invoke(sender, new EventArgs());
        }

        private static void FireGenerationStarted(object sender)
        {
            GenerationStarted?.Invoke(sender, new EventArgs());
        }

        private static void FireGenerationCompleted(object sender)
        {
            GenerationCompleted?.Invoke(sender, new EventArgs());
        }

        static SetGenerator()
        {
            recalculator = new BackgroundWorker();

            recalculator.DoWork += OnSettingsRecalculationStarted;
            recalculator.RunWorkerCompleted += OnSettingsRecalculationCompleted;

            generator.DoWork += OnGenerationStarted;
            generator.RunWorkerCompleted += OnGenerationCompleted;
        }
    }
}
