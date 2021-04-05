using XiangJiang.Core;
using XiangJiang.Orm.Abstractions;

namespace XiangJiang.Orm.EntityFrameworkCore
{
    public sealed class EfCoreUnitOfWork : IUnitOfWork
    {
        private readonly EfCoreDbContextBase _dbContext;

        public EfCoreUnitOfWork(EfCoreDbContextBase dbContext)
        {
            Checker.Begin().NotNull(dbContext, nameof(dbContext));
            _dbContext = dbContext;
        }

        public void BeginTransaction()
        {
            if (!TransactionEnabled)
                _dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (TransactionEnabled)
                _dbContext.Database.CurrentTransaction.Commit();
        }

        public void Rollback()
        {
            if (TransactionEnabled)
                _dbContext.Database.CurrentTransaction.Rollback();
        }

        public bool TransactionEnabled => _dbContext.Database.CurrentTransaction != null;
    }
}