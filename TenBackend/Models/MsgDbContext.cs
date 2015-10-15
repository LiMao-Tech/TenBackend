using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenBackend.Models
{
    public class MsgDbContext : DbContext
    {
        public MsgDbContext()
            : base("TenUserDbConnection")
        {
        }

        public System.Data.Entity.DbSet<TenBackend.Models.Msg> Msgs { get; set; }
    }
}