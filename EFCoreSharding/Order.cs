using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShardingCore.Core;

namespace EFCoreSharding
{
    /// <summary>
    /// 订单表
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 付款用户名
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 付款金额分
        /// </summary>
        public long Money { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
