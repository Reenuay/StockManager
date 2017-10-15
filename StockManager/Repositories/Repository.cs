using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using StockManager.Models;

namespace StockManager.Repositories
{
    class Repository<TEntity> : IRepository<TEntity> where TEntity : Base
    {
        protected Context context = new Context();

        public virtual bool AutoSaveChanges { get; set; }
        public virtual bool AutoSaveSuspended { get; protected set; } = false;

        public virtual DbSet<TEntity> Set
        {
            get
            {
                return context.Set<TEntity>();
            }
        }

        public Repository(bool autoSaveChanges = true)
        {
            AutoSaveChanges = autoSaveChanges;
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public virtual List<TEntity> Select(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).ToList();
        }

        public virtual List<TEntity> SelectAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public virtual void Insert(TEntity item)
        {
            context.Set<TEntity>().Add(item);

            if (AutoSaveChanges)
                context.SaveChanges();
        }

        public virtual void Update(TEntity item)
        {
            item.DateChanged = DateTime.Now;
            context.Entry(item).State = EntityState.Modified;

            if (AutoSaveChanges)
                context.SaveChanges();
        }

        public virtual void Delete(TEntity item)
        {
            context.Set<TEntity>().Remove(item);

            if (AutoSaveChanges)
                context.SaveChanges();
        }

        public void SuspendAutoSave()
        {
            AutoSaveChanges = false;
            AutoSaveSuspended = true;
        }

        public virtual void SaveChanges()
        {
            context.SaveChanges();

            if (AutoSaveSuspended)
            {
                AutoSaveChanges = true;
                AutoSaveSuspended = false;
            }
        }
    }
}
