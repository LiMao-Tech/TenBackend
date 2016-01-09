using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TenBackend.Models.Entities;

namespace TenBackend.Models.Assitants
{
    public class UserDetails
    {
        public TenLogin TenLogin { set; get; }
        public TenUser TenUser { set; get; }
        public int ProfileID { set; get; }
        public int PhotoCount { set; get; }

    }
}