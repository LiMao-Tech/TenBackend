using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class TenLogin
    {
        [Key]
        public int LoginIndex { get; set; }

        public int UserIndex { get; set; }

        [Required]
        [StringLength(32)]
        public string UserID { get; set; }

        [Required]
        [StringLength(32)]
        public string UserPWD { get; set; }

        public DateTime LastLogin { get; set; }

        [Required]
        public string DeviceUUID { get; set; }

        [Required]
        [MaxLength(64)]
        public string DeviceToken { get; set; }

        [Required]
        [MaxLength(64)]
        public string HashValue { get; set; }
    }
}