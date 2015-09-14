using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace TenBackend.Models
{
    [Table("limao.TenUsers")]
    public class TenUser
    {
        [Key]
        public int user_index { get; set; }

        [Required]
        [StringLength(32)]
        public string user_id { get; set; }

        [Required]
        [StringLength(32)]
        public string user_name { get; set; }

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
        public string quote { get; set; }

        public double? latitude { get; set; }

        public double? longitude { get; set; }
    }

    public class TenUserDbContext : DbContext {

        public TenUserDbContext() : base("TenUserDbConnection")
        {
        }

        public System.Data.Entity.DbSet<TenBackend.Models.TenUser> TenUsers { get; set; }
    }
}