using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XiangJiang.Orm.Abstractions;

namespace XiangJiang.Orm.EntityFrameworkCoreTests.Models
{
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
}