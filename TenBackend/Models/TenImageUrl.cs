namespace TenBackend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("limao.ImageUrls")]
    public partial class TenImageUrl
    {
        [Key]
        public int image_url_index { get; set; }

        public int user_index { get; set; }

        public DateTime image_added_datetime { get; set; }

        [Required]
        [StringLength(256)]
        public string image_url { get; set; }
    }
}
