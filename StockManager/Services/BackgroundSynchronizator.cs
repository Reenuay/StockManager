using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NLog;
using StockManager.Models;
using StockManager.Utilities;

namespace StockManager.Services
{
    /// <summary>
    /// Предоставляет сервис автоматической синхронизации базы фонов с папкой.
    /// </summary>
    static class BackgroundSynchronizator
    {
        #region Fields

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static BackgroundWorker synchronizator;
        private static FileSystemWatcher watcher;

        private static Context context = new Context();

        #endregion

        #region Events

        /// <summary>
        /// Выстреливает когда начинается синхронизация.
        /// </summary>
        public static event SynchronizatorEventHandler SyncStarted;

        /// <summary>
        /// Выстреливает когда синхронизация завершена.
        /// </summary>
        public static event SynchronizatorEventHandler SyncCompleted;

        /// <summary>
        /// Выстреливает когда меняется состояние синхронизации сервиса.
        /// </summary>
        public static event SynchronizatorEventHandler SyncStateChanged;

        #endregion

        #region Methods

        #region Watcher

        private static void OnCreatedOrChanged(object sender, FileSystemEventArgs e)
        {
            if (!BackgroundDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            if (HashGenerator.TryFileToMD5(e.FullPath, out string hash))
            {
                var repo = App.GetRepository<Background>(context);

                repo.ExecuteTransaction(() =>
                {
                    // Помечаем все фоны с данным путём как удалённые
                    foreach (var b in repo.Select(b => b.FullPath == e.FullPath))
                    {
                        if (!b.IsDeleted)
                        {
                            b.IsDeleted = true;
                            repo.Update(b);
                        }
                    }

                    // Ищем фон с полученной чек-суммой
                    Background background = repo.Find(i => i.CheckSum == hash);

                    if (background == null)
                    {
                        // Добавляем новый
                        repo.Insert(new Background
                        {
                            FullPath = e.FullPath,
                            CheckSum = hash,
                            IsDeleted = false
                        });
                    }
                    else
                    {
                        // Или обновляем уже существующий
                        background.FullPath = e.FullPath;
                        background.IsDeleted = false;
                        repo.Update(background);
                    }
                });
            }

            FireSyncCompleted(sender);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (!BackgroundDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            var repo = App.GetRepository<Background>(context);

            repo.ExecuteTransaction(() =>
            {
                // Помечаем все фоны с данным путём как удалённые
                foreach (var b in repo.Select(b => b.FullPath == e.FullPath))
                {
                    if (!b.IsDeleted)
                    {
                        b.IsDeleted = true;
                        repo.Update(b);
                    }
                }
            });

            FireSyncCompleted(sender);
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!BackgroundDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            var repo = App.GetRepository<Background>(context);

            Background background = repo.Find(b =>
                b.FullPath == e.OldFullPath
                && b.IsDeleted == false
            );

            if (background != null)
            {
                background.FullPath = e.FullPath;
                repo.Update(background);
            }
            else
            {
                logger.Warn(
                    $"File {e.OldFullPath} was renamed to {e.FullPath}"
                    + " but non-deleted background with that name does not exists"
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

            var repo = App.GetRepository<Background>(context);

            repo.ExecuteTransaction(() =>
            {
                // Собираем все чек-суммы и пути в один словарь.
                var pathOf = new Dictionary<string, string>();

                foreach (var fileName in BackgroundDirectory.GetBackgrounds())
                {
                    if (HashGenerator.TryFileToMD5(fileName, out string hash))
                    {
                        pathOf[hash] = fileName;
                    }
                }

                // Сверяем фоны в базе со словарём.
                foreach (var background in repo.SelectAll())
                {
                    if (pathOf.TryGetValue(background.CheckSum, out string path))
                    {
                        pathOf.Remove(background.CheckSum);
                        background.IsDeleted = false;
                        background.FullPath = path;
                        repo.Update(background);
                    }
                    else
                    {
                        background.IsDeleted = true;
                        repo.Update(background);
                    }
                }

                // Значения, которые остались в словаре - добавляем в базу.
                foreach (KeyValuePair<string, string> entry in pathOf)
                {
                    repo.Insert(new Background
                    {
                        FullPath = entry.Value,
                        CheckSum = entry.Key,
                        IsDeleted = false
                    });
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
            var e = new SynchronizatorEventArgs(SyncState.Started);
            SyncStarted?.Invoke(sender, e);
            SyncStateChanged?.Invoke(sender, e);
        }

        private static void FireSyncCompleted(object sender)
        {
            var e = new SynchronizatorEventArgs(SyncState.Completed);
            SyncCompleted?.Invoke(sender, e);
            SyncStateChanged?.Invoke(sender, e);
        }

        #endregion

        static BackgroundSynchronizator()
        {
            // Проверяем папку иконок и создаём, если её нет.
            if (!BackgroundDirectory.Exists())
            {
                BackgroundDirectory.Create();
                logger.Debug(
                    "Directory for backgrounds created at "
                    + BackgroundDirectory.FullPath
                );
            }

            // Инициализируем сервис фоновой загрузки.
            synchronizator = new BackgroundWorker();

            // Настраиваем сервис фоновой загрузки.
            synchronizator.DoWork += Sync;
            synchronizator.RunWorkerCompleted += OnSyncCompleted;

            // Инициализируем наблюдатель.
            watcher = BackgroundDirectory.CreateWatcher();

            watcher.Created += OnCreatedOrChanged;
            watcher.Changed += OnCreatedOrChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            // Подгружаем фоны из папки.
            RequestSynchronization();
        }
    }
}
