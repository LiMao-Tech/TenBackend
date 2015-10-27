namespace TenBackend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("limao.TenUsers")]
    public partial class TenUser
    {
        [Key]
        public int user_index { get; set; }

        [Required]
        [StringLength(32)]
        public string user_id { get; set; }

        [Required]
        [StringLength(32)]
        public string user_pw { get; set; }

        [Required]
        [StringLength(32)]
        public string user_name { get; set; }

        [Required]
        [MaxLength(16)]
        public byte[] device_uuid { get; set; }

        [Required]
        [MaxLength(64)]
        public byte[] device_token { get; set; }

        public byte phone_type { get; set; }

        public byte gender { get; set; }

        [Column(TypeName = "date")]
        public DateTime birth_date { get; set; }

        [Column(TypeName = "date")]
        public DateTime joined_date { get; set; }

        public DateTime last_login_datetime { get; set; }

        [Column(TypeName = "money")]
        public decimal p_coin { get; set; }

        public int outer_score { get; set; }

        public int inner_score { get; set; }

        public int energy { get; set; }

        [StringLength(128)]
        public string hobby { get; set; }

        [StringLength(128)]
        public string quote { get; set; }

        public double? latitude { get; set; }

        public double? longitude { get; set; }
    }
}
