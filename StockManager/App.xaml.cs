using System.Windows;
using StockManager.Models;
using StockManager.Repositories;

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
    }
}
