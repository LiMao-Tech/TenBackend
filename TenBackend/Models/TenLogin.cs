namespace TenBackend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("limao.TenLogins")]
    public partial class TenLogin
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
        [MaxLength(16)]
        public byte[] DeviceUUID { get; set; }

        [Required]
        [MaxLength(64)]
        public byte[] DeviceToken { get; set; }

        [Required]
        [MaxLength(64)]
        public byte[] HashValue { get; set; }
    }
}
