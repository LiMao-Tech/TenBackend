using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class ImageUnlocker
    {
        [Key]
        public int ID { set; get; }
        /// <summary>
        /// 图片ID
        /// </summary>
        public int TenImageID { set; get; }
        /// <summary>
        /// 图片拥有者UserIndex
        /// </summary>
        public int Owner { set; get; }
        /// <summary>
        /// 解锁人UserIndex
        /// </summary>
        public int Unlocker { set; get; }
        /// <summary>
        /// 解锁花费的P币
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Pcoin { set; get; }
        /// <summary>
        /// 解锁时间
        /// </summary>
        public long UnlockTime { set; get; }
    }
}