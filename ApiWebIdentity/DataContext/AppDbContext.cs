using ApiWebIdentity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiWebIdentity.DataContext
{
    public class AppDbContext : IdentityDbContext //Retirar o DbContext para o Identity
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):
            base(options)
        {
        }

        public DbSet<Hero> Heroes { get; set; }

        //Retirando quando subistituiu o DbContext pelo IdentityDbContext
        #region Comment
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Hero>().HasData(
        //        new Hero
        //        { 
        //            Id = 1,
        //            CivilName = "Peter Parker",
        //            HeroName = "Spider-Man",
        //            Email = "peter@gmail.com",
        //            Age = 28,
        //            City = "New York City"
        //        }
        //        );
        //}

        #endregion
    }
}
