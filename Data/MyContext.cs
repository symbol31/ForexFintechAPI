using ForexFintechAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForexFintechAPI.Data
{
    public class MyContext : IdentityDbContext
    {
        public MyContext(DbContextOptions<MyContext> options): base(options)
        {

        }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ExchangeManipulationData> ExchangeManipulationData { get; set; }
        public DbSet<LimitingData> LimitingData { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ExchangeRate>()
                .Property(e => e.Exchange_Rate)
                .HasColumnType("decimal(10,6)");
            builder.Entity<ExchangeRate>()
                .Property(e => e.Service_Fee)
                .HasColumnType("decimal(10,2)");
            builder.Entity<ExchangeManipulationData>()
                .Property(e => e.Margin)
                .HasColumnType("decimal(10,3)");
            builder.Entity<ExchangeManipulationData>()
                .Property(e => e.Discount)
                .HasColumnType("decimal(10,3)");
            builder.Entity<ExchangeManipulationData>()
                .Property(e => e.Premium)
                .HasColumnType("decimal(10,3)");
            builder.Entity<ExchangeManipulationData>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(10,3)");
            builder.Entity<ExchangeManipulationData>()
                .Property(e => e.Rate)
                .HasColumnType("decimal(10,3)");
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            });
        }
    }

}
