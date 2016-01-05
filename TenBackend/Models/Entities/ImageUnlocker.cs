using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class ImageUnlocker
    {
        [Key]
        public int ID { set; get; }
        public int TenImageID { set; get; }
        public int Owner { set; get; }
        public int Unlocker { set; get; }
        [Column(TypeName = "money")]
        public decimal Pcoin { set; get; }
        [Column(TypeName = "date")]
        public DateTime UnlockTime { set; get; }
    }
}