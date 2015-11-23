using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models.DbContexts
{
    public class TenImageDbContext : DbContext
    {
        public TenImageDbContext()
            : base("TenDbConnection")
        {

        }

        public DbSet<TenImage> TenImages { get; set; }
    }
}