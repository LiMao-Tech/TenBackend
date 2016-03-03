using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities.Kefu
{
    public class NotificationInfo
    {
        public int ID { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }

        public string Achor { get; set; }
    }
}