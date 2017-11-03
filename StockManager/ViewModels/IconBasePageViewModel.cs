using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using NLog;
using PropertyChanged;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Repositories;
using StockManager.Services;

namespace StockManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class IconBasePageViewModel
    {
        #region Fields

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Context context = new Context();

        #endregion

        #region Properties

        #region IconList

        public List<Icon> IconList { get; private set; }

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
                        if (IsSyncComplete)
                        {
                            IsSyncComplete = false;
                            return;
                        }

                        IconSynchronizator.RequestSynchronization();
                    }
                );
            }
        }

        public bool IsSyncing { get; private set; }
        public bool IsSyncComplete { get; private set; }

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

        #region AddKeywordsCommand

        public ICommand AddKeywordsCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        var keywordNames
                            = (KeywordsText ?? "").ToLower().Split(
                                new string[]{ "\r\n", "\r", "\n" },
                                StringSplitOptions.RemoveEmptyEntries
                            ).Distinct();

                        var newKeywordNames
                            = from keywordName
                              in keywordNames
                              where IconInfo.Keywords.All(k =>
                              {
                                  return k.Name != keywordName;
                              })
                              select keywordName;

                        var repo = new Repository<Keyword>(context);

                        repo.ExecuteTransaction(() =>
                        {
                            foreach (var newKeywordName in newKeywordNames)
                            {
                                var keyword
                                    = repo.Find(k => k.Name == newKeywordName);

                                if (keyword == null)
                                {
                                    keyword = new Keyword
                                    {
                                        Name = newKeywordName
                                    };

                                    repo.Insert(keyword);
                                }

                                keyword.Icons.Add(IconInfo.Icon);
                            }
                        });

                        KeywordsText = "";
                    }
                );
            }
        }

        public string KeywordsText { get; set; }

        #endregion

        #endregion

        #region Methods

        private void RefreshIconList()
        {
            IconList = new Repository<Icon>(context).SelectAll();
        }

        #endregion

        public IconBasePageViewModel()
        {
            IconList = new List<Icon>();
            IconInfo = new IconViewModel();

            IconSynchronizator.SyncStateChanged += (sender, e) =>
            {
                IsSyncing = e.State == SyncState.Started ? true : false;

                if (e.State == SyncState.Started)
                {
                    IconInfo = new IconViewModel();
                }
                else
                {
                    IsSyncComplete = true;
                    RefreshIconList();
                }
            };
        }
    }
}
