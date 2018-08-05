using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using AngleSharp.Parser.Html;
using RestSharp;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Utilities;
using Z.EntityFramework.Plus;

namespace StockManager.ViewModels {
    class KeyworderPageViewModel : SingletonViewModelBase<KeyworderPageViewModel> {

        private const string MicrostockgroupURL = "http://microstockgroup.com/tools/keyword.php";

        private int maximum = 50;
        private int haveLessOrEqualTo = 50;
        private int afterEachNNames = 0;
        private bool stopRequested = false;

        private Timer timer;
        private Context context = new Context();
        private DateTime keywordingStartTime = DateTime.Now;
        private BackgroundWorker worker = new BackgroundWorker();

        public DateTime Now { get; set; } = DateTime.Now;
        public string Percentage { get; set; } = "0%";
        public string Message { get; set; }
        public double Average { get; set; }
        public bool IsKeywording { get; set; }
        public int Total { get; set; }

        public int Maximum {
            get {
                return maximum;
            }
            set {
                if (value > 0)
                    maximum = value;
            }
        }
        public int HaveLessOrEqualTo {
            get {
                return haveLessOrEqualTo;
            }
            set {
                if (value >= 0)
                    haveLessOrEqualTo = value;
            }
        }
        public int AfterEachNNames {
            get {
                return afterEachNNames;
            }
            set {
                if (value >= 0)
                    afterEachNNames = value;
            }
        }
        public string TimeElapsed {
            get {
                return (Now - keywordingStartTime).ToString(@"dd\:hh\:mm\:ss");
            }
        }

        public string AverageText {
            get {
                return Average.ToString("F2");
            }
        }

        public ICommand StartKeywordingCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (!worker.IsBusy) {
                            IsKeywording = true;
                            keywordingStartTime = DateTime.Now;
                            Total = 0;
                            Average = 0;
                            timer = new Timer(
                                new TimerCallback((arg) => {
                                    Now = DateTime.Now;
                                }),
                                null,
                                0,
                                1000
                            );
                            ClearMessages();
                            worker.RunWorkerAsync();
                        }
                    }
                );
            }
        }

        public ICommand StopKeywordingCommand {
            get {
                return new RelayCommand(
                    o => {
                        stopRequested = true;
                        AddMessage("Stop requested...");
                    }
                );
            }
        }

        public KeyworderPageViewModel() {
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += OnWorkCompleted;
        }

        private void DoWork(object sender, DoWorkEventArgs e) {
            Process();
        }

        private void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e) {
            IsKeywording = false;
            timer.Dispose();
            timer = null;
            stopRequested = false;
        }

        private void AddMessage(string message = "") {
            Message = Message.TakeFirstLines(299);
            Message = message + "\n" + Message;

            if (!string.IsNullOrEmpty(message))
                Logger.Debug(message);
        }

        private void ClearMessages() {
            Message = "";
        }

        private void LogException(Exception ex) {
            if (ex != null) {
                AddMessage(ex.Message);
                AddMessage(ex.StackTrace);
                LogException(ex.InnerException);
            }
        }

        private void Process() {
            try {
                context = new Context();

                AddMessage($"Started process at {keywordingStartTime}");
                AddMessage($"Requesting {MicrostockgroupURL}");

                var client = new RestClient(MicrostockgroupURL);
                var parser = new HtmlParser();
                var rgx = new Regex("[^a-zA-Z]");
                var maxRequests = 30;

                AddMessage("Getting icons...");

                var nameIconGrouppings = context.Icons
                    .Where(i =>
                        HaveLessOrEqualTo == 0
                        || i.IconKeywords.Count < HaveLessOrEqualTo
                    )
                    .Select(i => new IconSlice {
                        Id = i.Id,
                        FullPath = i.FullPath,
                        LastPriority = i.IconKeywords
                            .Select(ik => ik.Priority)
                            .OrderByDescending(p => p)
                            .Take(1)
                            .FirstOrDefault()
                    })
                    .ToList()
                    .GroupBy(i => rgx.Replace(
                            Path.GetFileNameWithoutExtension(i.FullPath),
                            " "
                        ).Trim()
                    );

                AddMessage($"Icon count - {nameIconGrouppings.Sum(ig => ig.Count())}");

                AddMessage("Icon names to go:");
                foreach (var name in nameIconGrouppings) {
                    AddMessage($" - {name.Key} has {name.Count()} icons");
                }

                foreach (var groupping in nameIconGrouppings) {

                    AddMessage();
                    AddMessage("------------------START-------------------");

                    var startTime = DateTime.Now;

                    var retryCount = 0;
                    var iconGroup = groupping.ToList();

                    IRestResponse response = null;

                    AddMessage($"Keywording for {groupping.Key}...");

                    while (true) {
                        if (retryCount++ > maxRequests) {
                            AddMessage("Problems with server. Stopping process...");
                            stopRequested = true;
                            break;
                        }

                        if (stopRequested)
                            break;

                        AddMessage($"Requesting images. Try number {retryCount}");

                        var request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddCookie("language", "en");
                        request.AddCookie("num_results", "10");
                        request.AddCookie("image_type", "photo");
                        request.AddParameter(
                            "application/x-www-form-urlencoded",
                            $"search_term={groupping.Key}&image_type=photo&language=en&num_results=10", ParameterType.RequestBody
                        );
                        response = client.Execute(request);

                        AddMessage($"Response - {response.StatusCode}");

                        if (response.StatusCode.ToString() == "OK")
                            break;
                    }

                    if (stopRequested)
                        break;

                    var document = parser.ParseAsync(
                        response?.Content
                    ).Result;

                    AddMessage("Getting images. Selector - '.singleCell img'");

                    var imgIds = document
                        .QuerySelectorAll(".singleCell img")
                        .Select(img => img.Id)
                        .ToList();

                    AddMessage($"Got {imgIds.Count} imgs...");

                    AddMessage("Requesting keywords...");

                    retryCount = 0;
                    while (true) {
                        if (retryCount++ > maxRequests) {
                            AddMessage("Problems with server. Stopping process...");
                            stopRequested = true;
                            break;
                        }

                        if (stopRequested)
                            break;

                        AddMessage($"Requesting keywords. Try number {retryCount}");

                        var imageids = string.Join(
                            "&",
                            imgIds.Select(id => "imageid[]=" + id)
                        );

                        var request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddCookie("language", "en");
                        request.AddCookie("num_results", "10");
                        request.AddCookie("image_type", "photo");
                        request.AddParameter("application/x-www-form-urlencoded", imageids, ParameterType.RequestBody);
                        response = client.Execute(request);

                        AddMessage($"Response - {response.StatusCode}");

                        if (response.StatusCode.ToString() == "OK")
                            break;
                    }

                    if (stopRequested)
                        break;

                    document = parser.ParseAsync(
                        response?.Content
                    ).Result;

                    AddMessage("Getting keywords. Selector - '.keywordDisplay'");
                    var keywords = document
                        .QuerySelectorAll(".keywordDisplay")
                        .Select(k => k.Id)
                        .ToList();

                    AddMessage($"Got {keywords.Count} keywords");
                    foreach (var keyword in keywords) {
                        AddMessage($" - {keyword}");
                    }

                    keywords.Add(groupping.Key);

                    keywords = keywords
                        .Distinct()
                        .OrderBy(
                            k => k,
                            Comparer<string>.Create((a, b) => {
                                return Convert.ToInt32(a == groupping.Key)
                                    - Convert.ToInt32(b == groupping.Key);
                            })
                        )
                        .Take(Maximum)
                        .ToList();

                    AddMessage("Keywords to go:");
                    foreach (var key in keywords) {
                        AddMessage($" - {key}");
                    }

                    AddMessage("Cleaning previous keywords...");
                    var ids = iconGroup.Select(ig => ig.Id).ToList();
                    var deleted = context.IconKeywords
                        .Where(ik => ids.Contains(ik.IconId))
                        .Delete();

                    AddMessage($"{deleted} keywords were deleted from {groupping.Key}");

                    foreach (var key in keywords) {

                        if (stopRequested)
                            break;

                        var keyword = context.Keywords
                            .Where(k => k.Name == key)
                            .Take(1)
                            .FirstOrDefault();

                        if (keyword == null) {
                            keyword = new Keyword {
                                Name = key
                            };

                            context.Keywords.Add(keyword);
                            context.SaveChanges();
                        }

                        foreach (var icon in iconGroup) {

                            if (stopRequested)
                                break;

                            context.IconKeywords.Add(new IconKeyword {
                                IconId = icon.Id,
                                KeywordId = keyword.Id,
                                Priority = ++icon.LastPriority,
                            });
                        }
                    }

                    AddMessage("Saving changes to database...");
                    context.SaveChanges();

                    if (AfterEachNNames != 0 && Total > 0 && Total % AfterEachNNames == 0) {
                        AddMessage("Refreshing memory...");
                        context = new Context();
                    }

                    var endTime = DateTime.Now;

                    AddMessage($"Time elapsed {(endTime - startTime).ToString()}");
                    AddMessage("----------------FINISH------------------");
                    AddMessage();

                    Average = (
                        Average * Total
                        + (endTime - startTime).TotalSeconds
                    )
                    / (++Total);

                    var wordsCount = nameIconGrouppings.Count();

                    Percentage = ((double)Total / wordsCount * 100).ToString("F2")
                        + "% ("
                        + Total
                        + " / "
                        + wordsCount
                        + ")";
                }
            }
            catch (Exception ex) {
                LogException(ex);
                AddMessage("------------------ERROR-------------------");
                AddMessage();
            }
        }
    }

    class IconSlice {
        public int Id { get; set; }
        public string FullPath { get; set; }
        public int LastPriority { get; set; }
    }
}
