using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class BindInfo
    {
        [Key]
        public int ID{set;get;}
        public int LoginIndex { set; get; }
        public int PhoneNumber{set;get;}
        public bool PhoneState {set;get;}
        public string EmailAddress{set;get;}
        public DateTime EmailTime { set; get; }
        public string ValidateStr { set; get; }
        public bool EmailState{set;get;}

    }
}