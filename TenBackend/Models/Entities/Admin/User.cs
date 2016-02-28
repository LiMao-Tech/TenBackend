using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities.Admin
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string username { get; set; }
        public string userpsd { get; set; }
        public int role { get; set; }
    }
}