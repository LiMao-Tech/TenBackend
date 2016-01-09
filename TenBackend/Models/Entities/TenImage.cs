using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TenBackend.Models.Assitants;

namespace TenBackend.Models.Entities
{
    public class TenImage
    {
        [Key]
        public int ID { set; get; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        [StringLength(255)]
        public string BasePath { get; set; }
        public bool IsLocked { get; set; }
        public ImageType ImageType { get; set; }
        [Column(TypeName = "date")]
        public DateTime UploadTime { get; set; }
        public int UserIndex { get; set; }
        public int MsgIndex { get; set; }
    }
}