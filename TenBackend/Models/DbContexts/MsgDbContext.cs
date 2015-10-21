using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models.DbContexts
{
    public class MsgDbContext : DbContext
    {
        public MsgDbContext()
            : base("TenDbConnection")
        {
        }

        public DbSet<Msg> Msgs { get; set; }
    }
}