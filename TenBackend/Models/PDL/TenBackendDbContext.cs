using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models.PDL
{
    public class TenBackendDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public TenBackendDbContext()
            : base("name=TenBackendContext")
        {
        }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.TenLogin> TenLogins { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.TenUser> TenUsers { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.TenImage> TenImages { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.TenMsg> TenMsgs { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.PCoinTrans> PCoinTrans { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.ImageUnlocker> ImageUnlockers { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.Rater> Raters { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.Entities.BindInfo> BindInfoes { get; set; }

      
    }
}
