namespace TenBackend.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PcoinTransDbContext : DbContext
    {
        public PcoinTransDbContext()
            : base("TenUserDbConnection")
        {

        }

        public virtual DbSet<PcoinTrans> PcoinTrans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PcoinTrans>()
                .Property(e => e.pcoin_amount)
                .HasPrecision(19, 4);
        }
    }
}
