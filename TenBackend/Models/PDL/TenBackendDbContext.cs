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

        public System.Data.Entity.DbSet<TenBackend.Models.TenLogin> TenLogins { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.TenUser> TenUsers { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.TenImage> TenImages { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.TenMsg> TenMsgs { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.PCoinTrans> PCoinTrans { get; set; }

        public System.Data.Entity.DbSet<TenBackend.Models.ImageUnlocker> ImageUnlockers { get; set; }
    
    }
}
