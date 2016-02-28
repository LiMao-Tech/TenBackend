using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class Purchase
    {
        [Key]
        public int ID { get; set; }
        public string UserId { get; set; }
        public long PurchaseDate { get; set; }
        public long ModifiedDate { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public byte PurchaseType { get; set; }
    }
}