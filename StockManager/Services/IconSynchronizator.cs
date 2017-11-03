using System;
using System.ComponentModel;
using System.IO;
using NLog;
using StockManager.Models;
using StockManager.Repositories;
using StockManager.Utilities;

namespace StockManager.Services
{
    /// <summary>
    /// Предоставляет сервис автоматической синхронизации базы иконок с папкой.
    /// </summary>
    static class IconSynchronizator
    {
        #region Fields

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static BackgroundWorker synchronizator;
        private static FileSystemWatcher watcher;

        #endregion

        #region Events

        /// <summary>
        /// Выстреливает когда начинается синхронизация.
        /// </summary>
        public static event IconSynchronizatorEventHandler SyncStarted;

        /// <summary>
        /// Выстреливает когда синхронизация завершена.
        /// </summary>
        public static event IconSynchronizatorEventHandler SyncCompleted;

        /// <summary>
        /// Выстреливает когда меняется состояние синхронизации сервиса.
        /// </summary>
        public static event IconSynchronizatorEventHandler SyncStateChanged;

        #endregion

        #region Methods

        #region Watcher

        private static void OnCreatedOrChanged(object sender, FileSystemEventArgs e)
        {
            if (!IconDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            if (HashGenerator.TryFileToMD5(e.FullPath, out string hash))
            {
                var iconRepo = new Repository<Icon>();

                iconRepo.ExecuteTransaction(() =>
                {
                    // Помечаем все иконки с данным путём как удалённые
                    iconRepo.Select(i => i.FullPath == e.FullPath)
                        .ForEach(i =>
                        {
                            i.IsDeleted = true;
                        });

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
                });
            }

            FireSyncCompleted(sender);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (!IconDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            var iconRepo = new Repository<Icon>();

            iconRepo.ExecuteTransaction(() =>
            {
                // Помечаем все иконки с данным путём как удалённые
                iconRepo.Select(i => i.FullPath == e.FullPath)
                .ForEach(i =>
                {
                    i.IsDeleted = true;
                    iconRepo.Update(i);
                });
            });

            FireSyncCompleted(sender);
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!IconDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            var iconRepo = new Repository<Icon>();

            Icon icon = iconRepo.Find(i =>
                i.FullPath == e.OldFullPath
                && i.IsDeleted == false
            );

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

            FireSyncCompleted(sender);
        }

        #endregion

        #region Synchronizator

        /// <summary>
        /// Запрашивает синхронизацию.
        /// </summary>
        public static void RequestSynchronization()
        {
            if (!synchronizator.IsBusy)
            {
                synchronizator.RunWorkerAsync();
            }
        }

        private static void Sync(object sender, DoWorkEventArgs e)
        {
            FireSyncStarted(sender);

            var iconRepo = new Repository<Icon>();

            iconRepo.ExecuteTransaction(() =>
            {
                // Помечаем все иконки как удалённые.
                iconRepo.SelectAll()
                .ForEach(i =>
                {
                    i.IsDeleted = true;
                    iconRepo.Update(i);
                });

                // Пробегаемся по файлам в директории.
                foreach (var iconFile in IconDirectory.GetIcons())
                {
                    if (HashGenerator.TryFileToMD5(iconFile, out string hash))
                    {
                        // Ищем иконку с полученной чек-суммой.
                        Icon icon = iconRepo.Find(i => i.CheckSum == hash);

                        if (icon == null)
                        {
                            // Добавляем новую.
                            iconRepo.Insert(new Icon
                            {
                                FullPath = iconFile,
                                CheckSum = hash,
                                IsDeleted = false
                            });
                        }
                        else
                        {
                            // Или обновляем уже существующую.
                            icon.FullPath = iconFile;
                            icon.IsDeleted = false;
                            iconRepo.Update(icon);
                        }
                    }
                }
            });
        }

        private static void OnSyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FireSyncCompleted(sender);
        }

        #endregion

        private static void FireSyncStarted(object sender)
        {
            var e = new IconSynchronizatorEventArgs(SyncState.Started);
            SyncStarted?.Invoke(sender, e);
            SyncStateChanged?.Invoke(sender, e);
        }

        private static void FireSyncCompleted(object sender)
        {
            var e = new IconSynchronizatorEventArgs(SyncState.Completed);
            SyncCompleted?.Invoke(sender, e);
            SyncStateChanged?.Invoke(sender, e);
        }

        #endregion

        static IconSynchronizator()
        {
            // Проверяем папку иконок и создаём, если её нет.
            if (!IconDirectory.Exists())
            {
                IconDirectory.Create();
                logger.Debug(
                    "Directory for icons created at "
                    + IconDirectory.FullPath
                );
            }

            // Инициализируем сервис фоновой загрузки.
            synchronizator = new BackgroundWorker();

            // Настраиваем сервис фоновой загрузки.
            synchronizator.DoWork += Sync;
            synchronizator.RunWorkerCompleted += OnSyncCompleted;

            // Инициализируем наблюдатель.
            watcher = IconDirectory.CreateWatcher();

            watcher.Created += OnCreatedOrChanged;
            watcher.Changed += OnCreatedOrChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            // Подгружаем иконки из папки.
            RequestSynchronization();
        }
    }
}
