namespace TenBackend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;

    [Table("limao.Msgs")]
    public class Msg
    {
        [Key]
        public int msg_index { get; set; }

        public int msg_sender { get; set; }

        public int msg_receiver { get; set; }

        public bool msg_is_locked { get; set; }

        public byte msg_type { get; set; }

        public DateTime msg_time { get; set; }

        [Required]
        [StringLength(512)]
        public string msg_content { get; set; }
    }
}
