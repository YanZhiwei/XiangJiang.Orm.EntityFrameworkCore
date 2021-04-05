using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XiangJiang.Orm.Abstractions;
using XiangJiang.Orm.EntityFrameworkCore;
using XiangJiang.Orm.EntityFrameworkCoreTests.Models;

namespace XiangJiang.Orm.EntityFrameworkCoreTests
{
    [TestClass]
    public class SampleServiceTests
    {
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void SetUp()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<EfCoreDbContextBase, SampleDbContext>()
                .AddScoped<IUnitOfWork, EfCoreUnitOfWork>()
                .BuildServiceProvider();

            using (var dbContext = _serviceProvider.GetService<EfCoreDbContextBase>())
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
                dbContext.Create<Guid, EfSample>(new EfSample
                {
                    Id = Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C180"),
                    UserName = "yanzhiwei1"
                });
                dbContext.Create<Guid, EfSample>(new EfSample
                {
                    Id = Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C181"),
                    UserName = "yanzhiwei2"
                });
                dbContext.Create<Guid, EfSample>(new EfSample
                {
                    Id = Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C182"),
                    UserName = "yanzhiwei3"
                });
            }
        }


        [TestMethod]
        public void CURDTest()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<EfCoreDbContextBase>())
                {
                    var createActual = dbContext.Create<Guid, EfSample>(new EfSample
                    {
                        UserName = "yanzhiwei4"
                    });
                    Assert.IsTrue(createActual);

                    var actual =
                        dbContext.GetByKeyId<Guid, EfSample>(Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C180"));
                    Assert.IsNotNull(actual);
                    Assert.AreEqual("yanzhiwei1", actual.UserName);

                    actual.ModifyTime = DateTime.UtcNow;
                    var updateActual = dbContext.Update<Guid, EfSample>(actual);
                    Assert.IsTrue(updateActual);

                    var deleteActual = dbContext.Delete<Guid, EfSample>(actual);
                    Assert.IsTrue(deleteActual);
                }
            }
        }


        [TestMethod]
        public void TransactionSucceedTest()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<EfCoreDbContextBase>())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    try
                    {
                        unitOfWork.BeginTransaction();
                        var actual =
                            dbContext.GetByKeyId<Guid, EfSample>(Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C181"));
                        Assert.IsNotNull(actual);
                        Assert.AreEqual("yanzhiwei2", actual.UserName);

                        actual.ModifyTime = DateTime.UtcNow;
                        var updateActual = dbContext.Update<Guid, EfSample>(actual);
                        Assert.IsTrue(updateActual);

                        var deleteActual = dbContext.Delete<Guid, EfSample>(actual);
                        Assert.IsTrue(deleteActual);

                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                    }
                }
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<EfCoreDbContextBase>())
                {
                    var updateItem =
                        dbContext.GetByKeyId<Guid, EfSample>(Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C181"));
                    Assert.IsNull(updateItem);
                }
            }
        }


        [TestMethod]
        public void TransactionFailedTest()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<EfCoreDbContextBase>())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    unitOfWork.BeginTransaction();
                    var actual =
                        dbContext.GetByKeyId<Guid, EfSample>(Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C182"));
                    Assert.IsNotNull(actual);
                    Assert.AreEqual("yanzhiwei3", actual.UserName);

                    var deleteActual = dbContext.Delete<Guid, EfSample>(actual);
                    Assert.IsTrue(deleteActual);

                    unitOfWork.Rollback();
                }
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<EfCoreDbContextBase>())
                {
                    var updateItem =
                        dbContext.GetByKeyId<Guid, EfSample>(Guid.Parse("FFFDEC43-73DE-4D17-8764-8E7B56C6C182"));
                    Assert.IsNotNull(updateItem);
                }
            }
        }
    }
}