using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class Rater
    {
        [Key]
        public int ID { set; get; }
        public int RaterIndex { set; get; }
        public int UserIndex { set; get; }
        public int OuterScore { set; get; }
        public int InnerScore { set; get; }
        public int Energy { set; get; }
        public bool Active { set; get; }
    }
}