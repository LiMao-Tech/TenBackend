﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class UserDetails
    {
        public TenLogin TenLogin { set; get; }
        public TenUser TenUser { set; get; }
        public int ProfileID { set; get; }
        public int PhotoCount { set; get; }

    }
}