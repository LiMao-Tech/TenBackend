using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class TenLogin
    {
        /// <summary>
        /// 账户编号
        /// </summary>
        [Key]
        public int LoginIndex { get; set; }

        /// <summary>
        /// 个人信息编号
        /// </summary>
        public int UserIndex { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [Required]
        [StringLength(32)]
        public string UserID { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required]
        [StringLength(32)]
        public string UserPWD { get; set; }

        /// <summary>
        /// 上次登录的UNIX时间戳时间
        /// </summary>
        public long LastLogin { get; set; }

        /// <summary>
        /// IOS为DeviceUUID, Android为Mac
        /// </summary>
        [Required]
        public string DeviceUUID { get; set; }

        /// <summary>
        /// IOS为DeviceToken, Android为ClientId
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string DeviceToken { get; set; }

        /// <summary>
        /// 用于用户登录的Hash计算值
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string HashValue { get; set; }
    }
}