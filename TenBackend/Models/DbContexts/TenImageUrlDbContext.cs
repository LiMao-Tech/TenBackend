namespace TenBackend.Models.DbContexts
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class TenImageUrlDbContext : DbContext
    {
        public TenImageUrlDbContext()
            : base("TenDbConnection")
        {
        }

        public virtual DbSet<TenImageUrl> ImageUrls { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
