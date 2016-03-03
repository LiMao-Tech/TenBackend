using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities.Kefu
{
    public class UserListInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string JoinedTime { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public string Achor { get; set; }

    }
}