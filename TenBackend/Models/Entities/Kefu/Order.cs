using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities.Kefu
{
    public class Order
    {
        public string UserId { get; set; }
        public string Details { get; set; }
        public string OrderTime { get; set; }
        public string ModifiedTime { get; set; }
        public string Status { get; set; }
        public string OrderType { get; set; }
    }
}