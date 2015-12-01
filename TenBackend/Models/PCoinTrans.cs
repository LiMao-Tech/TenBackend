using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class PCoinTrans
    {
        [Key]
        public int ID{set;get;}

        [Required]
        public int Sender { get; set; }
        [Required]
        public int Receiver { get; set; }

        public byte PhoneType { get; set; }
        
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        public DateTime TransTime { get; set; }
        public string Note { get; set; }

        public PCoinTrans(int sender, int receiver,byte phonetype, decimal amount, DateTime transtime, string note)
        {
            Sender = sender;
            Receiver = receiver;
            PhoneType = phonetype;
            Amount = amount;
            TransTime = transtime;
            Note = note;
        }
    }
}