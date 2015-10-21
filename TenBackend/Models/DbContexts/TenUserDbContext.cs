using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models.DbContexts
{
    public class TenUserDbContext : DbContext
    {

        public TenUserDbContext()
            : base("TenDbConnection")
        {
        }
        public DbSet<TenUser> TenUsers { get; set; }
    }
}