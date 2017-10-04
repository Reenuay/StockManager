using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using NLog;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Repositories;
using StockManager.Utilities;

namespace StockManager.ViewModels
{
    class IconBasePageViewModel : INotifyPropertyChanged
    {
        #region Fields

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private FileSystemWatcher watcher;
        private BackgroundWorker backgroundLoader;
        private List<Icon> iconList = new List<Icon>();

        private ICommand syncCommand;
        private bool isSyncing;
        private bool isSyncComplete;

        #endregion

        #region Properties / Events

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

        public event PropertyChangedEventHandler PropertyChanged;

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

                            RunSync();
                        }
                    );

                return syncCommand;
            }
        }

        public bool IsSyncing
        {
            get
            {
                return isSyncing;
            }

            set
            {
                if (isSyncing != value)
                {
                    isSyncing = value;
                    NotifyPropertyChanged(nameof(IsSyncing));
                }
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

        #region Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }

        private bool TryGenerateHash(string fullPath, out string hash)
        {
            try
            {
                hash = HashGenerator.FileToMD5(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            hash = "";
            return false;
        }

        private void OnCreatedOrChanged(object source, FileSystemEventArgs e)
        {
            if (TryGenerateHash(e.FullPath, out string hash))
            {
                Repository<Icon> iconRepo = new Repository<Icon>();

                // Помечаем все иконки с данным путём как удалённые
                iconRepo.SuspendAutoSave();

                iconRepo.Select(i => i.FullPath == e.FullPath)
                    .ForEach(i => {
                        i.IsDeleted = true;
                        iconRepo.Update(i);
                    });

                iconRepo.SaveChanges();

                // Ищем иконку с полученной чек-суммой
                Icon icon = iconRepo.Find(i => i.CheckSum == hash);

                if (icon == null)
                {
                    // Добавляем новую
                    iconRepo.Insert(new Icon
                    {
                        FullPath = e.FullPath,
                        CheckSum = hash,
                        IsDeleted = false
                    });
                }
                else
                {
                    // Или обновляем уже существующую
                    icon.FullPath = e.FullPath;
                    icon.IsDeleted = false;
                    iconRepo.Update(icon);
                }

                RefreshIconList(iconRepo);
            }
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Repository<Icon> iconRepo = new Repository<Icon>();

            // Помечаем все иконки с данным путём как удалённые
            iconRepo.SuspendAutoSave();

            iconRepo.Select(i => i.FullPath == e.FullPath)
                .ForEach(i => {
                    i.IsDeleted = true;
                    iconRepo.Update(i);
                });

            iconRepo.SaveChanges();
            RefreshIconList(iconRepo);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Repository<Icon> iconRepo = new Repository<Icon>();

            Icon icon = iconRepo.Find(i =>
                i.FullPath == e.OldFullPath
                && i.IsDeleted == false);

            if (icon != null)
            {
                icon.FullPath = e.FullPath;
                iconRepo.Update(icon);
            }
            else
            {
                logger.Warn(
                    $"File {e.OldFullPath} was renamed to {e.FullPath}"
                    + " but non-deleted icon with that name does not exists"
                    + " in database."
                );
            }

            RefreshIconList(iconRepo);
        }

        private void RefreshIconList()
        {
            IconList = new Repository<Icon>().SelectAll();
        }

        private void RefreshIconList(Repository<Icon> iconRepo)
        {
            IconList = iconRepo.SelectAll();
        }

        private void Sync(object sender, DoWorkEventArgs e)
        {
            // Получаем локальную копию репозитория
            Repository<Icon> iconRepo = new Repository<Icon>();

            // Помечаем все иконки как удалённые
            iconRepo.SuspendAutoSave();

            iconRepo.SelectAll()
                .ForEach(i => {
                    i.IsDeleted = true;
                    iconRepo.Update(i);
                });

            // Пробегаемся по файлам в директории
            foreach (string iconFile in IconDirectory.GetIcons())
            {
                if (TryGenerateHash(iconFile, out string hash))
                {
                    // Ищем иконку с полученной чек-суммой
                    Icon icon = iconRepo.Find(i => i.CheckSum == hash);

                    if (icon == null)
                    {
                        // Добавляем новую
                        iconRepo.Insert(new Icon
                        {
                            FullPath = iconFile,
                            CheckSum = hash,
                            IsDeleted = false
                        });
                    }
                    else
                    {
                        // Или обновляем уже существующую
                        icon.FullPath = iconFile;
                        icon.IsDeleted = false;
                        iconRepo.Update(icon);
                    }
                }
            }

            iconRepo.SaveChanges();
        }

        private void SyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshIconList();
            IsSyncing = false;
            IsSyncComplete = true;
        }

        private void RunSync()
        {
            if (!backgroundLoader.IsBusy)
            {
                backgroundLoader.RunWorkerAsync();
                IsSyncing = true;
            }
        }

        #endregion

        public IconBasePageViewModel()
        {
            // Проверяем папку иконок и создаём, если её нет
            if (!IconDirectory.Exists())
            {
                IconDirectory.Create();
                logger.Debug(
                    "Directory for icons created: "
                    + IconDirectory.GetFullPath()
                );
            }

            // Инициализируем сервис фоновой загрузки
            backgroundLoader = new BackgroundWorker();

            // Настраиваем сервис фоновой загрузки
            backgroundLoader.DoWork += Sync;
            backgroundLoader.RunWorkerCompleted += SyncCompleted;

            // Инициализируем наблюдатель
            watcher = IconDirectory.CreateWatcher();

            watcher.Created += OnCreatedOrChanged;
            watcher.Changed += OnCreatedOrChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            RunSync();
        }
    }
}
