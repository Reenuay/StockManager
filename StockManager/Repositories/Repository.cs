using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using StockManager.Models;

namespace StockManager.Repositories
{
    /// <summary>
    /// Реализует методы обобщённого репозитория
    /// </summary>
    /// <typeparam name="TEntity">Сущность в репозитории</typeparam>
    class Repository<TEntity> : IRepository<TEntity> where TEntity : Base
    {
        private Context context = new Context();
        private bool autoCommit = true;

        /// <summary>
        /// Находит уникальную запись в репозитории по заданному условию
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Находит записи в репозитории, удовлетворяющие заданному условию
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        public List<TEntity> Select(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).ToList();
        }

        /// <summary>
        /// Возвращает все записи из репозитория
        /// </summary>
        public List<TEntity> SelectAll()
        {
            return context.Set<TEntity>().ToList();
        }

        /// <summary>
        /// Добавляем новый элемент в репозиторий
        /// </summary>
        /// <param name="item">Новый элемент</param>
        public void Insert(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            context.Set<TEntity>().Add(item);
            SaveChanges();
        }

        /// <summary>
        /// Обновляет существующий элемент в репозитории
        /// </summary>
        /// <param name="item">Обновляемый элемент</param>
        public void Update(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            context.Entry(item).State = EntityState.Modified;
            SaveChanges();
        }

        /// <summary>
        /// Удаляет элемент из репозитория
        /// </summary>
        /// <param name="item">Удаляемый элемент</param>
        public void Delete(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            context.Set<TEntity>().Remove(item);
            SaveChanges();
        }

        /// <summary>
        /// Выполняет транзакцию
        /// </summary>
        /// <param name="transaction">Транзакция</param>
        public void ExecuteTransaction(Action transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            autoCommit = false;
            transaction.Invoke();
            autoCommit = true;
            SaveChanges();
        }

        private void SaveChanges()
        {
            if (autoCommit)
                context.SaveChanges();
        }
    }
}
