using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StockManager.Models;
using System.Data.Entity;

namespace StockManager.Repositories
{
    interface IRepository<TEntity> where TEntity : Base
    {
        bool AutoSaveChanges { get; set; }
        DbSet<TEntity> Set { get; }

        TEntity Find(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> Select(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> SelectAll();

        void Insert(TEntity item);
        void Update(TEntity item);
        void Delete(TEntity item);

        void SuspendAutoSave();
        void SaveChanges();
    }
}
