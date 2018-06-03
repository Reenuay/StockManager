using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Services;
using System.Windows.Data;
using System.ComponentModel;

namespace StockManager.ViewModels
{
    class IconBasePageViewModel : ViewModelBase
    {
        #region Properties

        #region IconList

        public ObservableCollection<Icon> IconList { get; private set; }
        public ICollectionView FilteredIconList { get; private set; }
        public bool KeywordlessOnly { get; set; }

        public int ExistingIconsCount
        {
            get
            {
                return IconList.Count(i => !i.IsDeleted);
            }
        }

        public int DeletedIconsCount
        {
            get
            {
                return IconList.Count(i => i.IsDeleted);
            }
        }

        #endregion

        #region SyncCommand

        public ICommand SyncCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        IconSynchronizator.RequestSynchronization();
                    }
                );
            }
        }

        public bool IsSyncing { get; private set; }

        #endregion

        #region ShowInfoCommand

        public ICommand ShowInfoCommand
        {
            get
            {
                return new RelayCommand(
                    o => {
                        if (o is Icon i) {
                            IconInfo = new IconViewModel(i);
                        }
                    },
                    o => o is Icon
                );
            }
        }

        public bool AutoKeywordDone { get; set; } = true;
        public int Progress { get; set; }

        public ICommand AutoKeywordCommand {
            get {
                return new RelayCommand(
                    async o => {
                        AutoKeywordDone = false;

                        var icons = (
                            KeywordlessOnly
                                ? IconList.Where(i => !i.Keywords.Any())
                                : IconList.Where(i => i.Keywords.Count < 50)
                        );

                        await AutoKeywordService.DoWork(icons);

                        AutoKeywordDone = true;

                        RefreshIconList();
                    }
                );
            }
        }

        public IconViewModel IconInfo { get; private set; }

        #endregion

        #region AddKeywordCommand

        public ICommand AddKeywordCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        NewKeywordName = NewKeywordName.ToLower();

                        if (!string.IsNullOrEmpty(NewKeywordName))
                        {
                            if (IconInfo.Icon.Keywords.All(k => k.Name != NewKeywordName))
                            {
                                var repo = App.GetRepository<Keyword>();

                                Keyword keyword = repo.Find(k => k.Name == NewKeywordName);

                                repo.ExecuteTransaction(() =>
                                {
                                    if (keyword == null)
                                    {
                                        keyword = new Keyword
                                        {
                                            Name = NewKeywordName
                                        };

                                        repo.Insert(keyword);
                                    }
                                    else
                                    {
                                        repo.Update(keyword);
                                    }

                                    keyword.Icons.Add(IconInfo.Icon);

                                    foreach (var theme in ThemesList.Where(t => t.IsSelected).Select(t => t.Item))
                                    {
                                        if (!theme.Keywords.Contains(keyword))
                                        {
                                            theme.Keywords.Add(keyword);
                                        }
                                    }
                                });
                            }
                        }

                        NewKeywordName = "";
                    }
                );
            }
        }

        public string NewKeywordName { get; set; }

        #endregion

        #region RemoveKeywordCommand

        public ICommand RemoveKeywordCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (o is Keyword k)
                        {
                            var repo = App.GetRepository<Icon>();
                            IconInfo.Icon.Keywords.Remove(k);
                            repo.Update(IconInfo.Icon);
                        }
                    },
                    o => o is Keyword
                );
            }
        }

        #endregion

        #region ThemesList

        public ObservableCollection<SelectableListBoxItem<Theme>> ThemesList
        {
            get;
            set;
        }
        public ICollectionView OrderedThemesList { get; private set; }

        #endregion

        #endregion

        #region Methods

        private void RefreshIconList() {
            IconList = App.GetRepository<Icon>(new Context()).SelectAll();
            FilteredIconList = CollectionViewSource.GetDefaultView(IconList);
            FilteredIconList.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending)
            );
        }

        #endregion

        public IconBasePageViewModel()
        {
            IconList = new ObservableCollection<Icon>();
            ThemesList = new ObservableCollection<SelectableListBoxItem<Theme>>(
                App.GetRepository<Theme>().SelectAll().Select(
                    t => new SelectableListBoxItem<Theme>(t)
                )
            );

            IconInfo = new IconViewModel();
            IsSyncing = false;

            IconSynchronizator.SyncStateChanged += (sender, e) => {
                IsSyncing = e.State == SyncState.Started ? true : false;

                if (e.State == SyncState.Started) {
                    IconInfo = new IconViewModel();
                }
                else {
                    RefreshIconList();
                }
            };

            RefreshIconList();

            OrderedThemesList = CollectionViewSource.GetDefaultView(ThemesList);
            OrderedThemesList.SortDescriptions.Add(
                new SortDescription("Item.Name", ListSortDirection.Ascending)
            );

            PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                if (e.PropertyName == "KeywordlessOnly") {
                    if (KeywordlessOnly) {
                        FilteredIconList.Filter = i => !((Icon)i).Keywords.Any();
                    }
                    else {
                        FilteredIconList.Filter = null;
                    }
                }
            };

            AutoKeywordService.ProgressChanged += (sender, e) => {
                Progress = (int)(AutoKeywordService.Progress * 100);
            };
        }
    }
}
