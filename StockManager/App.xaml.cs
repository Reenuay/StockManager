using System;
using System.Windows;
using StockManager.Models;
using StockManager.Repositories;
using StockManager.Services;

namespace StockManager
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Context context = new Context();

        internal static Repository<TEntity> GetRepository<TEntity>() where TEntity: Base
        {
            return new Repository<TEntity>(context);
        }

        internal static Repository<TEntity> GetRepository<TEntity>(Context context) where TEntity : Base
        {
            return new Repository<TEntity>(context);
        }

        protected override void OnActivated(EventArgs e)
        {
            IconSynchronizator.RequestSynchronization();
            BackgroundSynchronizator.RequestSynchronization();
        }
    }
}
