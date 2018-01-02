﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Services;

namespace StockManager.ViewModels
{
    class IconBasePageViewModel : ViewModelBase
    {
        #region Properties

        #region IconList

        public ObservableCollection<Icon> IconList { get; private set; }

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
                    o =>
                    {
                        if (o is Icon i)
                        {
                            IconInfo = new IconViewModel(i);
                        }
                    },
                    o => o is Icon
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

        public ObservableCollection<SelectableListBoxItem<Theme>> ThemesList
        {
            get;
            set;
        }

        #endregion

        #region Methods

        private void RefreshIconList()
        {
            IconList = App.GetRepository<Icon>().SelectAll();
        }

        #endregion

        public IconBasePageViewModel()
        {
            IconList = new ObservableCollection<Icon>();
            IconInfo = new IconViewModel();
            IsSyncing = false;

            IconSynchronizator.SyncStateChanged += (sender, e) =>
            {
                IsSyncing = e.State == SyncState.Started ? true : false;

                if (e.State == SyncState.Started)
                {
                    IconInfo = new IconViewModel();
                }
                else
                {
                    RefreshIconList();
                }
            };

            RefreshIconList();

            ThemesList = new ObservableCollection<SelectableListBoxItem<Theme>>(
                App.GetRepository<Theme>().SelectAll().Select(
                    t => new SelectableListBoxItem<Theme>(t)
                )
            );
        }
    }
}
