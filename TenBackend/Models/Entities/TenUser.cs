using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class TenUser
    {
          [Key]
        public int UserIndex { get; set; }

        [Required]
        [StringLength(32)]
        public string UserName { get; set; }

        public byte PhoneType { get; set; }

        public byte Gender { get; set; }

        public byte Marriage { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthday { get; set; }

        [Column(TypeName = "date")]
        public DateTime JoinedDate { get; set; }

        [Column(TypeName = "money")]
        public decimal PCoin { get; set; }

        public int OuterScore { get; set; }

        public int InnerScore { get; set; }

        public int Energy { get; set; }

        [StringLength(256)]
        public string ProfileUrl { get; set; }

        [StringLength(128)]
        public string Hobby { get; set; }

        [StringLength(128)]
        public string Quote { get; set; }

        public double? Lati { get; set; }

        public double? Longi { get; set; }

    }
    }
