using ApiWebIdentity.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiWebIdentity.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):
            base(options)
        {
        }

        public DbSet<Hero> Heroes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Hero>().HasData(
                new Hero
                { 
                    Id = 1,
                    CivilName = "Peter Parker",
                    HeroName = "Spider-Man",
                    Email = "peter@gmail.com",
                    Age = 28,
                    City = "New York City"
                }
                );
        }
    }
}
