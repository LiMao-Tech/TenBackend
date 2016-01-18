using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class Rater
    {
        [Key]
        public int ID { set; get; }
        /// <summary>
        /// 评价者UserIndex
        /// </summary>
        public int RaterIndex { set; get; }
        /// <summary>
        /// 被评价者UserIndex
        /// </summary>
        public int UserIndex { set; get; }
        /// <summary>
        /// 外在,等于-1时表示不进行修改
        /// </summary>
        public int OuterScore { set; get; }
        /// <summary>
        /// 内在,等于-1时表示不进行修改
        /// </summary>
        public int InnerScore { set; get; }
        /// <summary>
        /// 能量,等于-1时表示不进行修改
        /// </summary>
        public int Energy { set; get; }
        /// <summary>
        /// 对话激活状态
        /// </summary>
        public bool Active { set; get; }
    }
}