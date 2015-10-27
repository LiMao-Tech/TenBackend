using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class TenLoginObj
    {
        // SHA-256 hash of "LiMao"
        public static string COMPANY_CODE = "e40cb24cffee7767d8f3bd9faf882af614b9e4bd402dc53a70f4723cde991734";

        public string user_id { get; set; }
        public string user_pw { get; set; }
        public string device_uuid { get; set; }
        public string device_token { get; set; }
        public string timestamp { get; set; }

        public string hash_result { get; set; }
    }
}