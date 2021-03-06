﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenBackend.Models
{
    public class TenImage
    {
        public int ID { set; get; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        [StringLength(255)]
        public string BasePath { get; set; }
        public ImageType ImageType { get; set; }
        [Column(TypeName = "date")]
        public DateTime UploadTime { get; set; }
        public int UserIndex { get; set; }
        public int MsgIndex { get; set; }
    }
}