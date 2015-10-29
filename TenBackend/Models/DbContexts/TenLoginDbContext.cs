using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models.DbContexts
{
    public class TenLoginDbContext : DbContext
    {
        public TenLoginDbContext()
            : base("TenDbConnection")
        {
        }
        public DbSet<TenLogin> TenLogins { get; set; }
    }
}