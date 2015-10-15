using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class TenUserDbContext : DbContext
    {

        public TenUserDbContext()
            : base("TenUserDbConnection")
        {
        }
        public DbSet<TenUser> TenUsers { get; set; }
    }
}