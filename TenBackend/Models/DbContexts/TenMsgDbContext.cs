using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models.DbContexts
{
    public class TenMsgDbContext : DbContext
    {

        public TenMsgDbContext()
            : base("TenDbConnection")
        {
        }
        public DbSet<TenMsg> TenMsgs { get; set; }
    }
}