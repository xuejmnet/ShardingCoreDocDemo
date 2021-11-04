using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;

namespace EFCoreSharding
{
    public class MyDbContext: AbstractShardingDbContext,IShardingTableDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>(o =>
            {
                o.HasKey(p => p.Id);
                o.Property(p => p.Id).IsRequired().IsUnicode(false).HasMaxLength(40).HasComment("订单Id");
                o.Property(p => p.Payer).IsRequired().HasMaxLength(50).HasComment("付款用户名");
                o.Property(p => p.Money).HasComment("付款金额分");
                o.Property(p => p.CreateTime).HasComment("创建时间");
                o.Property(p => p.IsDelete).HasComment("是否已删除");
                o.HasQueryFilter(p => p.IsDelete == false);
                o.ToTable(nameof(Order));
            });
        }

        public IRouteTail RouteTail { get; set; }
    }
}
