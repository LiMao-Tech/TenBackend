namespace TenBackend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("limao.TenMsgs")]
    public class TenMsg
    {
        [Key]
        public int MsgIndex { get; set; }

        public int Sender { get; set; }

        public int Receiver { get; set; }

        public byte PhoneType { get; set; }

        public bool IsLocked { get; set; }

        public byte MsgType { get; set; }

        public DateTime MsgTime { get; set; }

        [Required]
        [StringLength(512)]
        public string MsgContent { get; set; }
    }
}
