using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StockManager.Repositories
{
    /// <summary>
    /// Предоставляет обощённые CRUD операции для взаимодействия с репозиторием
    /// </summary>
    /// <typeparam name="TEntity">Сущность в репозитории</typeparam>
    interface IRepository<TEntity>
    {
        /// <summary>
        /// Находит уникальную запись в репозитории по заданному условию
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Находит записи в репозитории, удовлетворяющие заданному условию
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        List<TEntity> Select(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Возвращает все записи из репозитория
        /// </summary>
        List<TEntity> SelectAll();

        /// <summary>
        /// Добавляем новый элемент в репозиторий
        /// </summary>
        /// <param name="item">Новый элемент</param>
        void Insert(TEntity item);

        /// <summary>
        /// Обновляет существующий элемент в репозитории
        /// </summary>
        /// <param name="item">Обновляемый элемент</param>
        void Update(TEntity item);

        /// <summary>
        /// Удаляет элемент из репозитория
        /// </summary>
        /// <param name="item">Удаляемый элемент</param>
        void Delete(TEntity item);

        /// <summary>
        /// Выполняет транзакцию
        /// </summary>
        /// <param name="transaction">Транзакция</param>
        void ExecuteTransaction(Action transaction);
    }
}
