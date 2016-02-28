using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class TenUser
    {
          [Key]
        public int UserIndex { get; set; }

          /// <summary>
          /// 姓名
          /// </summary>
        [Required]
        [StringLength(32)]
        public string UserName { get; set; }

        /// <summary>
        /// 电话类型，0为iphone,1为Android
        /// </summary>
        public byte PhoneType { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public byte Marriage { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public long Birthday { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        
        public long JoinedDate { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public long Expire { get; set; }

        /// <summary>
        /// 购买到的等级
        /// </summary>
        public int AVG { get; set; }

        /// <summary>
        /// P币
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PCoin { get; set; }

        /// <summary>
        /// 外在
        /// </summary>
        public int OuterScore { get; set; }

        /// <summary>
        /// 内在
        /// </summary>
        public int InnerScore { get; set; }

        /// <summary>
        /// 能量
        /// </summary>
        public int Energy { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        [StringLength(256)]
        public string ProfileUrl { get; set; }

        /// <summary>
        /// 爱好
        /// </summary>
        [StringLength(128)]
        public string Hobby { get; set; }

        /// <summary>
        /// 简述
        /// </summary>
        [StringLength(128)]
        public string Quote { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lati { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Longi { get; set; }

    }
    }
