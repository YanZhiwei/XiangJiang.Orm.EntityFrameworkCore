# XiangJiang.Orm.EntityFrameworkCore

[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/XiangJiang.Orm.EntityFrameworkCore.svg)](https://www.nuget.org/packages/XiangJiang.Orm.EntityFrameworkCore) [![nuget](https://img.shields.io/nuget/dt/XiangJiang.Orm.EntityFrameworkCore.svg)](https://www.nuget.org/packages/XiangJiang.Orm.EntityFrameworkCore)
包含事务单元，仓储模式的 EntityFrameworkCore 实现

喜欢这个项目的话就 Star、Fork、Follow
项目开发模式：日常代码积累+网络搜集

## 本项目已得到[JetBrains](https://www.jetbrains.com/shop/eform/opensource)的支持！

<img src="https://www.jetbrains.com/shop/static/images/jetbrains-logo-inv.svg" height="100">

## 请注意：

一旦使用本开源项目以及引用了本项目或包含本项目代码的公司因为违反劳动法（包括但不限定非法裁员、超时用工、雇佣童工等）在任何法律诉讼中败诉的，项目作者有权利追讨本项目的使用费，或者直接不允许使用任何包含本项目的源代码！任何性质的`外包公司`或`996公司`需要使用本类库，请联系作者进行商业授权！其他企业或个人可随意使用不受限。

## 建议开发环境

操作系统：Windows 10 1903 及以上版本  
开发工具：VisualStudio2019 v16.9 及以上版本  
SDK：.Net 5.0 及以上版本

## 安装程序包

.NET ≥ 5.0

```shell
PM> Install-Package XiangJiang.Orm.EntityFrameworkCore
```

## 构建 DbContext

只需要继承实现 EfCoreDbContextBase 即可，即可完成 DbContext 构建

```csharp
public sealed class SampleDbContext : EfCoreDbContextBase
{
    public DbSet<EfSample> Samples { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=EfCoreSample.db3;");
    }
}
```

## 创建 Entity

只需要派生于 ModelBase，并设置 Entity 主键类型

```csharp
[Table("EFSample")]
[Description("EF 测试表")]
public sealed class EfSample : ModelBase<Guid>
{
    public EfSample()
    {
        Id = Guid.NewGuid();
        CreateTime = DateTime.UtcNow;
        ModifyTime = DateTime.MinValue;
        Available = true;
    }

    [MaxLength(20)]
    [Display(Name = "User Name")]
    public string UserName { get; set; } // nvarchar(20), null
}
```

### CURD 操作

```csharp
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
```

### 3.事务操作

```csharp
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
```
