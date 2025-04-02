using Microsoft.EntityFrameworkCore;
using TestRazor.Models;

namespace TestRazor.Data
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(DefaultDataProvider.GetDefaultCategories());
        }
    }
}
