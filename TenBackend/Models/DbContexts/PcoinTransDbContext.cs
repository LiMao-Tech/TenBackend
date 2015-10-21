namespace TenBackend.Models.DbContexts
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class PcoinTransDbContext : DbContext
    {
        public PcoinTransDbContext()
            : base("TenDbConnection")
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
