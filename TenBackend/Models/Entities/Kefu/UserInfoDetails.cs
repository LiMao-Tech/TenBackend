using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities.Kefu
{
    public class UserInfoDetails
    {
        public string UserProfile { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Marriage { get; set; }
        public string Birthday { get; set; }
        public decimal PCoin { get; set; }
        public int Level { get; set; }
        public string Location { get; set; }
        public string PhoneType { get; set; }
        public string Status { get; set; }
        public string JoinedTime { get; set; }
        public string LastLogin { get; set; }
        public string Hobby { get; set; }
        public string Quote { get; set; }

        public int UserIndex { get; set; }
    }
}