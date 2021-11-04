using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreShardingTable.Entities;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.VirtualRoutes.Mods;

namespace EFCoreShardingTable.VirtualRoutes
{
    public class SysUserVirtualTableRoute:AbstractSimpleShardingModKeyStringVirtualTableRoute<SysUser>
    {
        public SysUserVirtualTableRoute() : base(2, 3)
        {
        }

        public override void Configure(EntityMetadataTableBuilder<SysUser> builder)
        {
            builder.ShardingProperty(o => o.Id);
        }
    }
}
