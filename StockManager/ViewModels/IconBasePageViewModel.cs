using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ImageMagick;
using NLog;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Repositories;
using StockManager.Services;

namespace StockManager.ViewModels
{
    class IconBasePageViewModel : INotifyPropertyChanged
    {
        #region Fields

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private List<Icon> iconList = new List<Icon>();
        private IconInfo iconInfo = new IconInfo();
        private ICommand showInfoCommand;
        private ICommand syncCommand;
        private bool isSyncComplete;

        #endregion

        #region Properties

        #region IconList

        public List<Icon> IconList
        {
            get
            {
                return iconList;
            }

            private set
            {
                if (iconList != value)
                {
                    iconList = value;
                    NotifyPropertyChanged(nameof(IconList));
                    NotifyPropertyChanged(nameof(ExistingIconsCount));
                    NotifyPropertyChanged(nameof(DeletedIconsCount));
                }
            }
        }

        public int ExistingIconsCount
        {
            get
            {
                return iconList.Count(i => !i.IsDeleted);
            }
        }

        public int DeletedIconsCount
        {
            get
            {
                return iconList.Count(i => i.IsDeleted);
            }
        }

        #endregion

        #region SyncCommand

        public ICommand SyncCommand
        {
            get
            {
                syncCommand = syncCommand
                    ?? new RelayCommand(
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

                return syncCommand;
            }
        }

        public bool IsSyncing
        {
            get
            {
                return IconSynchronizator.IsSynchronizing;
            }
        }

        public bool IsSyncComplete
        {
            get
            {
                return isSyncComplete;
            }

            set
            {
                if (isSyncComplete != value)
                {
                    isSyncComplete = value;
                    NotifyPropertyChanged(nameof(IsSyncComplete));
                }
            }
        }

        #endregion

        #region ShowPreviewCommand

        public ICommand ShowInfoCommand
        {
            get
            {
                showInfoCommand = showInfoCommand
                    ?? new RelayCommand(
                        o =>
                        {
                            if (o is Icon i)
                            {
                                var info = new IconInfo
                                {
                                    RelativePath = i.FullPath.Remove(
                                        0,
                                        Environment.CurrentDirectory.Length + 1
                                    ),
                                    CheckSum = i.CheckSum,
                                    Date = i.DateCreated.ToString("dd.MM.yyyy"),
                                    Keywords = i.Keywords
                                };

                                if (!i.IsDeleted)
                                {
                                    using (
                                        var image
                                            = new MagickImage(i.FullPath)
                                    )
                                    {
                                        info.Preview = image.ToBitmapSource();
                                    }

                                    info.Size = ((
                                        new FileInfo(i.FullPath)
                                    ).Length / 1024).ToString() + " KB";
                                }

                                IconInfo = info;
                            }
                        },
                        o => o is Icon
                    );

                return showInfoCommand;
            }
        }

        public IconInfo IconInfo
        {
            get
            {
                return iconInfo;
            }

            set
            {
                if (iconInfo != value)
                {
                    iconInfo = value;
                    NotifyPropertyChanged(nameof(IconInfo));
                }
            }
        }

        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }

        private void RefreshIconList()
        {
            IconList = new Repository<Icon>().SelectAll();
        }

        #endregion

        public IconBasePageViewModel()
        {
            IconSynchronizator.SyncStarted += (sender, e) =>
            {
                NotifyPropertyChanged(nameof(IsSyncing));
            };

            IconSynchronizator.SyncCompleted += (sender, e) =>
            {
                IsSyncComplete = true;
                NotifyPropertyChanged(nameof(IsSyncing));
                RefreshIconList();
            };
        }
    }
}
