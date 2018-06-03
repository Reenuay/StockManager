using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Services;

namespace StockManager.ViewModels
{
    class GeneratorPageViewModel : ViewModelBase
    {
        private bool stopRequested = false;

        public ObservableCollection<Theme> Themes { get; private set; }
            = new ObservableCollection<Theme>();

        public ObservableCollection<Template> Templates { get; private set; }
            = new ObservableCollection<Template>();

        public ObservableCollection<Background> Backgrounds { get; private set; }
            = new ObservableCollection<Background>();

        public Theme Theme { get; set; }
        public Template Template { get; set; }
        public Background Background { get; set; }
        public int Percentage { get; set; } = 50;
        public int Maximum { get; set; }
        public bool IsWorking { get; private set; } = SetGenerator.IsWorking;
        public int MatchingIcons { get; private set; }
        public BigInteger UsedCombinations { get; private set; } = SetGenerator.ExistingCombinationsCount;
        public string Log { get; private set; }

        public string NewQueueName { get; set; }
        public SelectableListBoxItem<Queue> CurrentQueue { get; set; }
        public ObservableCollection<SelectableListBoxItem<Queue>> QueuesList { get; private set; }
        public SelectableListBoxItem<Queue> ProcessingQueue { get; private set; }
        private int CurrentIndex { get; set; }

        public ICommand ProcessCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (SetGenerator.IsWorking)
                    {
                        stopRequested = true;
                        SetGenerator.StopOperations();
                    }
                    else
                    {
                        if (CurrentQueue == null)
                            return;

                        if (CurrentQueue.Item == null)
                            return;

                        if (CurrentQueue.Item.Tasks.Count == 0)
                            return;

                        ProcessingQueue = CurrentQueue;
                        ProcessingQueue.IsSelected = true;
                        IsWorking = true;

                        Task t = ProcessingQueue.Item.Tasks[CurrentIndex++];

                        bool can_pass()
                        {
                            return t != null
                                && t.Theme.Keywords.Count > 0
                                && t.Template.Cells.Count > 0;
                        }

                        while (!can_pass())
                        {
                            if (CurrentIndex < ProcessingQueue.Item.Tasks.Count)
                                t = ProcessingQueue.Item.Tasks[CurrentIndex++];
                            else
                                break;
                        }

                        if (t != null)
                        {
                            if (CurrentIndex == 0)
                                Log = "";

                            SetGenerator.SetSettingsAndStart(
                                t.Theme,
                                t.Template,
                                t.Background,
                                t.Percentage,
                                t.Maximum
                            );
                        }
                        else
                        {
                            ProcessingQueue.IsSelected = false;
                            ProcessingQueue = null;
                            IsWorking = false;
                            CurrentIndex = 0;
                        }
                    }
                });
            }
        }

        public ICommand NewQueueCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (string.IsNullOrEmpty(NewQueueName))
                        return;

                    if (QueuesList.Any(q => q.Item.Name == NewQueueName))
                        return;

                    App.GetRepository<Queue>().Insert(new Queue
                    {
                        Name = NewQueueName
                    });

                    QueuesList = new ObservableCollection<SelectableListBoxItem<Queue>>(
                        App.GetRepository<Queue>().SelectAll().Select(
                            q => new SelectableListBoxItem<Queue>(q)
                        )
                    );

                    NewQueueName = "";
                });
            }
        }

        public ICommand RemoveQueueCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (o is SelectableListBoxItem<Queue> q)
                    {
                        if (ProcessingQueue == null || q.Item == ProcessingQueue.Item)
                        {
                            if (IsWorking)
                                return;

                            ProcessingQueue = null;
                        }

                        App.GetRepository<Queue>().Delete(q.Item);

                        QueuesList = new ObservableCollection<SelectableListBoxItem<Queue>>(
                            App.GetRepository<Queue>().SelectAll().Select(
                                q2 => new SelectableListBoxItem<Queue>(q2)
                            )
                        );
                    }
                });
            }
        }

        public ICommand NewTaskCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (ProcessingQueue == CurrentQueue && IsWorking)
                        return;

                    if (Theme == null)
                        return;

                    if (Template == null)
                        return;

                    if (CurrentQueue == null)
                        return;

                    if (Percentage < 0 || Percentage > 100)
                        return;

                    if (Maximum < 0)
                        return;

                    App.GetRepository<Task>().Insert(new Task
                    {
                        Theme = Theme,
                        Template = Template,
                        Background = Background,
                        Percentage = Percentage,
                        Maximum = Maximum,
                        Queue = CurrentQueue.Item,
                    });
                });
            }
        }

        public ICommand RemoveTaskCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (ProcessingQueue == CurrentQueue && IsWorking)
                        return;

                    if (o is Task t)
                    {
                        App.GetRepository<Task>().Delete(t);
                    }
                });
            }
        }

        public GeneratorPageViewModel()
        {
            Themes = App.GetRepository<Theme>().SelectAll();
            Templates = App.GetRepository<Template>().SelectAll();
            Backgrounds = App.GetRepository<Background>().Select(b => !b.IsDeleted);

            if (Themes.Count > 0)
                Theme = Themes[0];

            if (Templates.Count > 0)
                Template = Templates[0];

            if (Backgrounds.Count > 0)
                Background = Backgrounds[0];

            if (SetGenerator.Theme != null)
                Theme = SetGenerator.Theme;

            if (SetGenerator.Template != null)
                Template = SetGenerator.Template;

            if (SetGenerator.Background != null)
                Background = SetGenerator.Background;

            MatchingIcons = SetGenerator.MatchingIcons == null
                ? 0
                : SetGenerator.MatchingIcons.Count;

            SetGenerator.SettingsRecalculationCompleted += (sender, e) =>
            {
                MatchingIcons = SetGenerator.MatchingIcons == null
                    ? 0
                    : SetGenerator.MatchingIcons.Count;
                UsedCombinations = SetGenerator.ExistingCombinationsCount;
            };

            SetGenerator.GenerationCompleted += (sender, e) =>
            {
                if (stopRequested)
                {
                    IsWorking = false;
                    ProcessingQueue.IsSelected = false;
                    ProcessingQueue = null;
                    stopRequested = false;
                    CurrentIndex = 0;
                    return;
                }

                if (ProcessingQueue?.Item.Tasks.Count > CurrentIndex) {
                    ProcessCommand.Execute(new object());
                }
                else
                {
                    CurrentIndex = 0;

                    if (ProcessingQueue != null)
                        ProcessingQueue.IsSelected = false;

                    ProcessingQueue = null;
                    IsWorking = false;
                }
            };

            SetGenerator.LogChanged += (sender, e) =>
            {
                Log += SetGenerator.ActionLog + "\n";
            };

            QueuesList = new ObservableCollection<SelectableListBoxItem<Queue>>(
                App.GetRepository<Queue>().SelectAll().Select(
                    q => new SelectableListBoxItem<Queue>(q)
                )
            );
        }
    }
}
