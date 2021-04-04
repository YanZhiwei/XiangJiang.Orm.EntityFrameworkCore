using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XiangJiang.Orm.Abstractions;

namespace XiangJiang.Orm.EntityFrameworkCore
{
    public sealed class EfCoreReadOnlyDbContext : DbContext, IReadonlyDbContext
    {
        public bool Exist<TPrimaryKey, TModelBase>(Expression<Func<TModelBase, bool>> predicate = null)
            where TModelBase : ModelBase<TPrimaryKey>
        {
            return predicate == null ? Set<TModelBase>().Any() : Set<TModelBase>().Any(predicate);
        }

        public async Task<bool> ExistAsync<TPrimaryKey, TModelBase>(Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = new()) where TModelBase : ModelBase<TPrimaryKey>
        {
            return await (predicate == null
                ? Set<TModelBase>().AnyAsync(token)
                : Set<TModelBase>().AnyAsync(predicate, token));
        }

        public TModelBase GetByKeyId<TPrimaryKey, TModelBase>(TPrimaryKey id) where TModelBase : ModelBase<TPrimaryKey>
        {
            var entity = Set<TModelBase>().Find(id);
            Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public async Task<TModelBase> GetByKeyIdAsync<TPrimaryKey, TModelBase>(TPrimaryKey id,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            var entity = Set<TModelBase>().FindAsync(id, token);
            Entry(entity).State = EntityState.Detached;
            return await entity;
        }

        public List<TModelBase> GetList<TPrimaryKey, TModelBase>(Expression<Func<TModelBase, bool>> predicate = null)
            where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return query.AsNoTracking().ToList();
        }

        public async Task<List<TModelBase>> GetListAsync<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = new()) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return await query.AsNoTracking().ToListAsync(token);
        }

        public TModelBase GetFirstOrDefault<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            return predicate != null
                ? query.AsNoTracking().FirstOrDefault(predicate)
                : query.AsNoTracking().FirstOrDefault();
        }

        public async Task<TModelBase> GetFirstOrDefaultAsync<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = default) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            return await (predicate != null
                ? query.AsNoTracking().FirstOrDefaultAsync(predicate, token)
                : query.AsNoTracking().FirstOrDefaultAsync(token));
        }

        public IQueryable<TModelBase> Query<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null) query = query.Where(predicate);

            return query.AsNoTracking();
        }

        public async Task<IQueryable<TModelBase>> QueryAsync<TPrimaryKey, TModelBase>(
            Expression<Func<TModelBase, bool>> predicate = null,
            CancellationToken token = new()) where TModelBase : ModelBase<TPrimaryKey>
        {
            IQueryable<TModelBase> query = Set<TModelBase>();

            if (predicate != null)
                query = query.Where(predicate);

            return await Task.Run(() => query.AsNoTracking(), token);
        }
    }
}