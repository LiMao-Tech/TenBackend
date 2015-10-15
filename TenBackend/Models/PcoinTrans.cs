namespace TenBackend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("limao.PcoinTrans")]
    public partial class PcoinTrans
    {
        [Key]
        public int pcoin_index { get; set; }

        public int pcoin_sender { get; set; }

        public int pcoin_receiver { get; set; }

        [Column(TypeName = "money")]
        public decimal pcoin_amount { get; set; }

        public DateTime pcoin_time { get; set; }

        [Required]
        [StringLength(128)]
        public string pcoin_note { get; set; }
    }
}
