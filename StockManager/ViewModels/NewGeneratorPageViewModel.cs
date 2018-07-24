using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Newtonsoft.Json;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Properties;
using StockManager.Services;
using StockManager.Utilities;

namespace StockManager.ViewModels {
    class NewGeneratorPageViewModel : ViewModelBase {
        private static Lazy<NewGeneratorPageViewModel> singleton
            = new Lazy<NewGeneratorPageViewModel>();

        public static NewGeneratorPageViewModel Singleton {
            get {
                singleton.Value.Refresh();
                return singleton.Value;
            }
        }

        private Context context = new Context();
        private DateTime startTime = DateTime.Now;
        private Timer timer;
        private int from = 1;
        private int to = 10;
        private int maximum = 0;
        private bool stopRequested = false;
        private BackgroundWorker worker = new BackgroundWorker();
        private FileSystemWatcher watcher;
        private EventWaitHandle generatorHandle
            = new EventWaitHandle(false, EventResetMode.AutoReset);
        private string waitForFileName;
        private string waitForFileNameWithoutExtension;

        public ObservableCollection<SelectableListBoxItem<Template>> TemplateList { get; private set; }
        public int Maximum {
            get {
                return maximum;
            }
            set {
                if (value >= 0)
                    maximum = value;
                else
                    Maximum = maximum;
            }
        }
        public string Keywords {
            get {
                return Settings.Default.GeneratorKeywords;
            }
            set {
                Settings.Default.GeneratorKeywords = value;
                Settings.Default.Save();
            }
        }
        public bool UseColors { get; set; } = true;
        public bool IsGenerating { get; set; }
        public int Total { get; set; }
        public DateTime Now { get; set; } = DateTime.Now;
        public string TimeElapsed {
            get {
                return (Now - startTime).ToString(@"dd\:hh\:mm\:ss");
            }
        }
        public bool UseRange { get; set; }
        public int From {
            get {
                return from;
            }
            set {
                if (value <= To && value > 0)
                    from = value;
                else
                    From = from;
            }
        }
        public int To {
            get {
                return to;
            }
            set {
                if (value >= From && value > 0)
                    to = value;
                else
                    To = to;
            }
        }
        public string Message { get; private set; } = "";

        public ICommand SelectTemplateCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (o is SelectableListBoxItem<Template> t) {
                            if (t.IsSelected) {
                                t.IsSelected = false;
                            }
                            else {
                                t.IsSelected = true;
                            }
                        }
                    }
                );
            }
        }

        public ICommand StartGenerationCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (!worker.IsBusy) {
                            IsGenerating = true;
                            startTime = DateTime.Now;
                            Total = 0;
                            timer = new Timer(
                                new TimerCallback((arg) => {
                                    Now = DateTime.Now;
                                }),
                                null,
                                0,
                                1000
                            );
                            Message = "";

                            worker.RunWorkerAsync();
                        }
                    }
                );
            }
        }

        public ICommand StopGenerationCommand {
            get {
                return new RelayCommand(
                    o => {
                        stopRequested = true;
                        generatorHandle.Set();
                        AddMessage("Stop requested...");
                    }
                );
            }
        }

        private void AddMessage(string message = "") {
            Message += message + "\n";
        }

        private void Refresh() {
            var selecteds = new Dictionary<int, bool>();
            if (TemplateList != null) {
                foreach (var t in TemplateList) {
                    selecteds[t.Item.Id] = t.IsSelected;
                }
            }

            TemplateList = new ObservableCollection<SelectableListBoxItem<Template>>(
                context
                .Templates
                .Include("Cells")
                .Where(t => !t.IsHidden && t.Cells.Any())
                .AsEnumerable()
                .Select(t =>
                    new SelectableListBoxItem<Template>(
                        t,
                        selecteds.ContainsKey(t.Id)
                            ? selecteds[t.Id]
                            : false
                    )
                )
            );
        }

        private void DoWork(object sender, DoWorkEventArgs e) {

            if (!File.Exists(Settings.Default.IllustratorPath)) {
                AddMessage("Please provide correct Illustrator.exe path.");
                return;
            }

            var processStartTime = DateTime.Now;
            AddMessage($"Started process at {processStartTime}");
            AddMessage("Processing keywords...");

            var setsPath = Path.Combine(
                Environment.CurrentDirectory,
                Settings.Default.SetsDirectory
            );

            var selectedKeywords = Keywords
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .Distinct()
                .ToList();

            if (selectedKeywords.Count == 0) {
                AddMessage("Error: No keywords provided");
                return;
            }

            AddMessage("Keywords provided:");
            foreach (var keyword in selectedKeywords) {
                AddMessage($"- {keyword}");
            }

            var keywords = context
                .Keywords
                .Include("Icons")
                .Where(k => selectedKeywords.Any(kw => kw == k.Name) && k.Icons.Any())
                .ToList();

            if (keywords.Count == 0) {
                AddMessage("Error: None of keywords has icons");
                return;
            }

            if (keywords.Count != selectedKeywords.Count) {
                AddMessage("Excluded keywords because of no icons:");
            }

            foreach (var keyword in selectedKeywords) {
                if (keywords.All(k => k.Name != keyword)) {
                    AddMessage($"- {keyword}");
                }
            }

            AddMessage("Passed keywords:");
            foreach (var keyword in keywords) {
                AddMessage($"- {keyword.Name}");
            }

            var selectedTemplates = TemplateList
                .Where(st => st.IsSelected)
                .Select(st => st.Item)
                .ToList();

            if (selectedTemplates.Count == 0) {
                AddMessage("Error: No templates provided");
                return;
            }

            var keywordTemplates = new Dictionary<Keyword, List<Template>>();

            foreach (var keyword in keywords) {
                var temps = new List<Template>();

                foreach (var template in selectedTemplates) {
                    if (keyword.Icons.Count >= template.Cells.Count) {
                        temps.Add(template);
                    }
                }

                if (temps.Count > 0) {
                    keywordTemplates.Add(keyword, temps);
                    AddMessage(
                        $"Keyword - {keyword.Name} matches with {temps.Count} template{(temps.Count == 1 ? "" : "s")}"
                    );

                    foreach (var temp in temps) {
                        AddMessage($" - {temp.Name}");
                    }
                }
                else {
                    AddMessage(
                        $"Keyword {keyword.Name} doesn't have enough icons - {keyword.Icons.Count} and will be excluded"
                    );
                }
            }

            if (keywordTemplates.Count == 0) {
                AddMessage("Error: None of keywords fits at least one of given templates");
                return;
            }

            AddMessage("Keywords to go:");
            foreach (var keyword in keywordTemplates.Keys) {
                AddMessage($"- {keyword.Name}");
            }

            var dice = new Random();
            var index = 0;
            var currentAmount = 0;
            var enumerator = new CombinationEnumerator();

            AddMessage($"Generation started at {DateTime.Now}");

            var newSetPath = Path.Combine(
                Environment.CurrentDirectory,
                Settings.Default.SetsDirectory,
                processStartTime.ToString(
                    "dd MMMM, yyyy (HH-mm-ss)"
                )
            );

            if (!Directory.Exists(newSetPath)) {
                AddMessage($"Directory created at {newSetPath}");
                Directory.CreateDirectory(newSetPath);
            }

            var code = Resources.Generator;

            while (Maximum == 0 || Total < Maximum) {
                try {
                    AddMessage($"Getting settings at {DateTime.Now}");

                    currentAmount = UseRange
                        ? From == To ? From : dice.Next(From, To)
                        : 10
                        ;

                    if (Maximum != 0 && currentAmount > Maximum - Total) {
                        currentAmount = Maximum - Total;
                    }

                    AddMessage($"Next range - {currentAmount}");

                    index = 0;

                    var keyword = keywordTemplates
                        .Keys
                        .Skip(dice.Next(0, keywordTemplates.Keys.Count))
                        .Take(1)
                        .Single();

                    AddMessage($"Keyword selected: {keyword.Name}");
                    AddMessage($"Icons count: {keyword.Icons.Count}");

                    var template = keywordTemplates[keyword]
                        .Skip(dice.Next(0, keywordTemplates[keyword].Count))
                        .Take(1)
                        .Single();

                    AddMessage($"Template selected: {template.Name}");
                    AddMessage($"Cells count: {template.Cells.Count}");

                    enumerator.ChangeValues(keyword.Icons.Count, template.Cells.Count);

                    AddMessage(
                        $"Possible combinations count on {keyword.Icons.Count}"
                        + $" icons and {template.Cells.Count} cells"
                        + $" is {enumerator.Count}"
                    );

                    while (index < currentAmount) {
                        if (stopRequested) {
                            break;
                        }

                        AddMessage();
                        AddMessage("------------------START-------------------");

                        var startTime = DateTime.Now;

                        AddMessage(
                            $"Set {index + 1} of {currentAmount} started at {startTime}"
                        );

                        var positions = enumerator.Random;

                        AddMessage("Trying new set:");

                        var combination = new ObservableCollection<Icon>();

                        for (var i = 0; i < enumerator.K; i++) {
                            var icon = keyword.Icons[positions[i]];

                            combination.Add(icon);

                            AddMessage($" - {icon.Name}");
                        }

                        var snapshot = HashGenerator.TextSequenceToMD5(
                            from i in combination select i.CheckSum
                        );

                        AddMessage($"Snapshot - {snapshot}");

                        var set = context
                            .Sets
                            .SingleOrDefault(s => s.Snapshot == snapshot);

                        if (set == null) {
                            AddMessage("Set not found. Generating...");

                            set = new Set {
                                Snapshot = snapshot,
                                Icons = combination,
                            };

                            context.Sets.Add(set);
                            context.SaveChanges();
                        }
                        else {
                            AddMessage("Set is already in database. Checking compositions...");

                            if (set.Compositions.Any()) {
                                AddMessage("Set has composition already, skipping...");
                            }

                            var skipTime = DateTime.Now;

                            AddMessage(
                                $"Set {index + 1} of {currentAmount} ended at {skipTime}"
                            );

                            AddMessage($"Time elapsed {(skipTime - startTime).ToString()}");

                            AddMessage("------------------SKIP--------------------");
                            AddMessage();

                            continue;
                        }

                        var randomOrders = set.Icons
                            .Select(p => dice.Next())
                            .ToArray();

                        var iconPositions = Enumerable
                            .Range(0, set.Icons.Count)
                            .OrderBy(i => randomOrders[i])
                            .ToArray();

                        var composition = new Composition {
                            Keywords = new ObservableCollection<Keyword>(
                                set.Icons.SelectMany(i => i.Keywords).Distinct()
                            ),
                            Theme = keyword,
                            Set = set,
                            Background = template.Backgrounds.Any(b => !b.IsDeleted)
                                ? template.Backgrounds
                                    .Where(b => !b.IsDeleted)
                                    .OrderBy(b => Guid.NewGuid())
                                    .Take(1)
                                    .First()
                                : context.Backgrounds
                                    .Where(b => !b.IsDeleted && !b.Templates.Any())
                                    .OrderBy(b => Guid.NewGuid())
                                    .Take(1)
                                    .First(),
                            Mappings = new ObservableCollection<Mapping>(
                                template.Cells.Join(
                                    set.Icons,
                                    c => template.Cells.IndexOf(c),
                                    i => iconPositions[set.Icons.IndexOf(i)],
                                    (c, i) => new Mapping {
                                        Cell = c,
                                        Icon = i
                                    }
                                )
                            ),
                        };

                        if (UseColors) {
                            var colorsCount
                                = composition.Background.Colors.Count;

                            if (colorsCount > 0) {
                                composition.Color
                                    = composition.Background.Colors[
                                        dice.Next(colorsCount)
                                    ];
                            }
                        }

                        composition.Name
                            = NameGenerator.GenerateName(composition);

                        AddMessage("New composition-----------------");
                        AddMessage($"Name - {composition.Name}");
                        AddMessage($"Color - #{composition.Color?.HEX}");
                        AddMessage($"Path - {composition.Background.FullPath}");
                        AddMessage($"Template - {composition.Template.Name}");

                        var iconJson = JsonConvert.SerializeObject(
                            composition.Mappings.Select(m => new {
                                icon = m.Icon.FullPath,
                                x = m.Cell.X,
                                y = m.Cell.Y,
                                w = m.Cell.Width,
                                h = m.Cell.Height,
                            })
                        );

                        var colorHEX = composition.Color?.HEX;

                        waitForFileName = Path.Combine(
                            newSetPath,
                            $"{composition.Set.Snapshot}.m.jpg"
                        );

                        waitForFileNameWithoutExtension = Path.Combine(
                            newSetPath,
                            composition.Set.Snapshot
                        );

                        var mustachioTemplate
                            = Mustachio.Parser.Parse(Resources.Generator);

                        dynamic model = new ExpandoObject();
                        model.epsName = Path.Combine(
                            newSetPath,
                            $"{composition.Set.Snapshot}.eps"
                        ).Replace(@"\", @"\\");
                        model.jpegName = waitForFileName.Replace(@"\", @"\\");
                        model.backgroundPath = composition
                            .Background
                            .FullPath
                            .Replace(@"\", @"\\");
                        model.color = colorHEX;
                        model.useColor = colorHEX == null ? 0 : 1;
                        model.icons = iconJson;
                        model.quitIllustrator = (
                            Settings.Default.RestartIllustrator
                            && Total > 0
                            && Total / Settings.Default.AfterEachNSets == 0
                        )
                        .ToString()
                        .ToLower();

                        AddMessage("Creating script...");

                        var content = mustachioTemplate(model);

                        var jsFilePath = Path.GetFullPath(
                            Path.Combine(
                                Environment.CurrentDirectory,
                                "script.js"
                            )
                        );

                        File.WriteAllText(
                            jsFilePath,
                            content
                        );

                        AddMessage("Calling Illustrator...");

                        var process = new Process();

                        process.StartInfo.FileName
                            = Settings.Default.IllustratorPath;

                        process.StartInfo.Arguments = jsFilePath;

                        process.Start();

                        generatorHandle.WaitOne(Settings.Default.WaitForIllustrator);

                        if (File.Exists(waitForFileName)) {
                            AddMessage("Writing Meta...");

                            IllustratorCaller.WriteMeta(
                                Path.Combine(
                                    newSetPath,
                                    composition.Set.Snapshot
                                ),
                                composition.Name,
                                composition.Keywords
                                    .Select(k => k.Name)
                            );

                            AddMessage("Saving changes to database...");

                            context.Compositions.Add(composition);
                            context.SaveChanges();
                        }
                        else {
                            AddMessage("Skipped...");
                        }

                        if (File.Exists(jsFilePath)) {
                            AddMessage("Deleting script...");
                            File.Delete(jsFilePath);
                        }

                        AddMessage("Ending...");

                        var endTime = DateTime.Now;

                        AddMessage(
                            $"Set {index + 1} of {currentAmount} ended at {endTime}"
                        );

                        AddMessage($"Time elapsed {(endTime - startTime).ToString()}");

                        AddMessage("------------------FINISH------------------");
                        AddMessage();

                        index++;
                        Total++;
                    }

                    if (stopRequested) {
                        stopRequested = false;
                        break;
                    }
                }
                catch (Exception ex) {

                    foreach(var ext in new[] { ".jpg", ".m.jpg", ".eps"} ) {
                        var fileName
                            = $"{waitForFileNameWithoutExtension}{ext}";

                        if (File.Exists(fileName)) {
                            if (WaitForFile(fileName, 5, 500))
                                File.Delete(fileName);
                        }
                    }

                    Logger.Error(ex);
                    LogException(ex);
                    AddMessage("------------------ERROR-------------------");
                    AddMessage();
                }
            }

            AddMessage($"Generation completed at {DateTime.Now}");
            AddMessage($"Total time elapsed {TimeElapsed}");
        }

        private void LogException(Exception ex) {
            if (ex != null) {
                AddMessage(ex.Message);
                AddMessage(ex.StackTrace);
                LogException(ex.InnerException);
            }
        }

        private void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e) {
            IsGenerating = false;
            timer.Dispose();
            timer = null;
        }

        private void OnCreated(object sender, FileSystemEventArgs e) {
            if (waitForFileName == e.FullPath) {
                Thread.Sleep(Settings.Default.WaitForIllustratorSavesFile);
                WaitForFile(
                    waitForFileName,
                    Settings.Default.WaitForFileTriesNumber,
                    Settings.Default.WaitForFileInterval
                );
                generatorHandle.Set();
            }
        }

        private bool WaitForFile(string fullPath, int tries, int interval) {
            var numTries = 0;
            while (true) {
                ++numTries;
                try {
                    using (
                        var fs = new FileStream(
                            fullPath,
                            FileMode.Open,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            100
                        )
                    ) {
                        fs.ReadByte();
                        break;
                    }
                }
                catch (Exception ex) {
                    AddMessage(
                       $"WaitForFile {fullPath} failed to get"
                       + $" an exclusive lock: {ex.ToString()}"
                    );

                    if (numTries > tries) {
                        return false;
                    }

                    Thread.Sleep(interval);
                }
            }

            AddMessage(
                $"WaitForFile {fullPath} returning true after {numTries} tries"
            );

            return true;
        }

        public NewGeneratorPageViewModel() {
            Refresh();

            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += OnWorkCompleted;

            var setsPath = Path.Combine(
                Environment.CurrentDirectory,
                Settings.Default.SetsDirectory
            );

            if (!Directory.Exists(setsPath))
                Directory.CreateDirectory(setsPath);

            watcher = new FileSystemWatcher {
                NotifyFilter = NotifyFilters.FileName,
                Path = setsPath,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            watcher.Created += OnCreated;
        }
    }
}
