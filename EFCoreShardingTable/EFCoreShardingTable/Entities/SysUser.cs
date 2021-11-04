using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreShardingTable.Entities
{
    /// <summary>
    /// user table
    /// </summary>
    public class SysUser
    {
        /// <summary>
        /// user id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// user name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// area
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// setting code
        /// </summary>
        public string SettingCode { get; set; }
    }
}
