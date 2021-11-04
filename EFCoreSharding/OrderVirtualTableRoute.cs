using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.VirtualRoutes.Mods;
using ShardingCore.VirtualRoutes.Years;

namespace EFCoreSharding
{
    /// <summary>
    /// 创建虚拟路由
    /// </summary>
    public class OrderVirtualTableRoute:AbstractSimpleShardingModKeyStringVirtualTableRoute<Order>
    {
        public OrderVirtualTableRoute() : base(2, 3)
        {
        }

        public override void Configure(EntityMetadataTableBuilder<Order> builder)
        {
            builder.ShardingProperty(o => o.Id);
            builder.AutoCreateTable(false);
            builder.TableSeparator("_");
        }
    }
}
