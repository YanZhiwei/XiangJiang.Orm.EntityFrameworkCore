using Microsoft.EntityFrameworkCore;
using XiangJiang.Orm.EntityFrameworkCore;
using XiangJiang.Orm.EntityFrameworkCoreTests.Models;

namespace XiangJiang.Orm.EntityFrameworkCoreTests
{
    public sealed class SampleDbContext : EfCoreDbContextBase
    {
        public DbSet<EfSample> Samples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=EfCoreSample.db3;");
        }
    }
}