using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NLog;
using StockManager.Models;
using StockManager.Utilities;
using System.Linq;

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
            if (!IconDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            if (HashGenerator.TryFileToMD5(e.FullPath, out string hash))
            {
                var repo = App.GetRepository<Icon>(context);

                repo.ExecuteTransaction(() =>
                {
                    // Помечаем все иконки с данным путём как удалённые
                    foreach (var i in repo.Select(i => i.FullPath == e.FullPath))
                    {
                        if (!i.IsDeleted)
                        {
                            i.IsDeleted = true;
                            repo.Update(i);
                        }
                    }

                    // Ищем иконку с полученной чек-суммой
                    Icon icon = repo.Find(i => i.CheckSum == hash);

                    if (icon == null)
                    {
                        // Добавляем новую
                        repo.Insert(new Icon
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
                        repo.Update(icon);
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

            var repo = App.GetRepository<Icon>(context);

            repo.ExecuteTransaction(() =>
            {
                var icons = repo.Select(i => i.FullPath == e.FullPath);
                var usedIcons = icons.Where(i =>
                    i.Sets.Any()
                );

                // Помечаем все использованные иконки с данным путём как удалённые
                foreach (var i in usedIcons)
                {
                    if (!i.IsDeleted)
                    {
                        i.IsDeleted = true;
                        repo.Update(i);
                    }
                }

                // Удаляем неиспользованные иконки из базы
                foreach (var i in icons.Except(usedIcons))
                {
                    repo.Delete(i);
                }
            });

            FireSyncCompleted(sender);
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!IconDirectory.ExtensionIsAllowed(e.FullPath))
                return;

            FireSyncStarted(sender);

            var repo = App.GetRepository<Icon>(context);

            Icon icon = repo.Find(i =>
                i.FullPath == e.OldFullPath
                && i.IsDeleted == false
            );

            if (icon != null)
            {
                icon.FullPath = e.FullPath;
                repo.Update(icon);
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

            var repo = App.GetRepository<Icon>(context);

            repo.ExecuteTransaction(() =>
            {
                // Собираем все чек-суммы и пути в один словарь.
                var pathOf = new Dictionary<string, string>();

                foreach (var fileName in IconDirectory.GetIcons())
                {
                    if (HashGenerator.TryFileToMD5(fileName, out string hash))
                    {
                        pathOf[hash] = fileName;
                    }
                }

                // Сверяем иконки в базе со словарём.
                foreach (var icon in repo.SelectAll())
                {
                    if (pathOf.TryGetValue(icon.CheckSum, out string path))
                    {
                        pathOf.Remove(icon.CheckSum);
                        icon.IsDeleted = false;
                        icon.FullPath = path;
                        repo.Update(icon);
                    }
                    else
                    {
                        icon.IsDeleted = true;
                        repo.Update(icon);
                    }
                }

                // Значения, которые остались в словаре - добавляем в базу.
                foreach (var entry in pathOf)
                {
                    repo.Insert(new Icon
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
