using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XiangJiang.Core;
using XiangJiang.Orm.Abstractions;

namespace XiangJiang.Orm.EntityFrameworkCore
{
    public sealed class EfCoreDbContext : DbContext, IDbContext
    {
        public bool Delete<TPrimaryKey, TModelBase>(TModelBase entity) where TModelBase : ModelBase<TPrimaryKey>
        {
            CheckNull<TPrimaryKey, TModelBase>(entity);
            Set<TModelBase>().Attach(entity);
            Set<TModelBase>().Remove(entity);
            return SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync<TPrimaryKey, TModelBase>(TModelBase entity,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            CheckNull<TPrimaryKey, TModelBase>(entity);
            Set<TModelBase>().Attach(entity);
            Set<TModelBase>().Remove(entity);
            return await SaveChangesAsync(token) > 0;
        }

        public bool Exist<TPrimaryKey, TModelBase>(Expression<Func<TModelBase, bool>> predicate = null)
            where TModelBase : ModelBase<TPrimaryKey>
        {
            return predicate == null ? Set<TModelBase>().Any() : Set<TModelBase>().Any(predicate);
        }

        public async Task<bool> ExistAsync<TPrimaryKey, TModelBase>(Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            return await (predicate == null
                ? Set<TModelBase>().AnyAsync(token)
                : Set<TModelBase>().AnyAsync(predicate, token));
        }

        public TModelBase GetByKeyId<TPrimaryKey, TModelBase>(TPrimaryKey id) where TModelBase : ModelBase<TPrimaryKey>
        {
            return Set<TModelBase>().Find(id);
        }

        public async Task<TModelBase> GetByKeyIdAsync<TPrimaryKey, TModelBase>(TPrimaryKey id,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            return await Set<TModelBase>().FindAsync(id);
        }

        public List<TModelBase> GetList<TPrimaryKey, TModelBase>(Expression<Func<TModelBase, bool>> predicate = null)
            where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return query.ToList();
        }

        public async Task<List<TModelBase>> GetListAsync<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return await query.ToListAsync(token);
        }

        public TModelBase GetFirstOrDefault<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            return predicate != null ? query.FirstOrDefault(predicate) : query.FirstOrDefault();
        }

        public Task<TModelBase> GetFirstOrDefaultAsync<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            return predicate != null ? query.FirstOrDefaultAsync(predicate, token) : query.FirstOrDefaultAsync(token);
        }

        public bool Create<TPrimaryKey, TModelBase>(TModelBase entity) where TModelBase : ModelBase<TPrimaryKey>
        {
            CheckNull<TPrimaryKey, TModelBase>(entity);
            Set<TModelBase>().Add(entity);
            return SaveChanges() > 0;
        }

        public async Task<bool> CreateAsync<TPrimaryKey, TModelBase>(TModelBase entity, CancellationToken token = new())
            where TModelBase : ModelBase<TPrimaryKey>
        {
            CheckNull<TPrimaryKey, TModelBase>(entity);
            await Set<TModelBase>().AddAsync(entity, token);
            return await SaveChangesAsync(token) > 0;
        }

        public IQueryable<TModelBase> Query<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return query;
        }

        public async Task<IQueryable<TModelBase>> QueryAsync<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = new()) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return await Task.Run(() => query, token);
        }

        public bool Update<TPrimaryKey, TModelBase>(TModelBase entity) where TModelBase : ModelBase<TPrimaryKey>
        {
            CheckNull<TPrimaryKey, TModelBase>(entity);
            var set = Set<TModelBase>();
            set.Attach(entity);
            Entry(entity).State = EntityState.Modified;
            return SaveChanges() > 0;
        }

        public async Task<bool> UpdateAsync<TPrimaryKey, TModelBase>(TModelBase entity,
            CancellationToken token = default)
            where TModelBase : ModelBase<TPrimaryKey>
        {
            CheckNull<TPrimaryKey, TModelBase>(entity);
            var set = Set<TModelBase>();
            set.Attach(entity);
            Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync(token) > 0;
        }

        private void CheckNull<TPrimaryKey, TModelBase>(TModelBase entity) where TModelBase : ModelBase<TPrimaryKey>
        {
            Checker.Begin().NotNull(entity, nameof(entity));
        }
    }
}