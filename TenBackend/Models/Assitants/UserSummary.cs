using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Assitants
{
    public class UserSummary
    {
        public int LoginIndex { get; set; }
        public string ProfileUrl { get; set; }
        public string UserId { get; set; }
        public DateTime LastLogin { get; set; }
        public string DeviceUUID { get; set; }
        public string DeviceToken { get; set; }
        
    }
}