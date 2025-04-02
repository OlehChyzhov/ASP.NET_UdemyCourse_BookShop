using BookShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Data
{
    public class ShopContext : IdentityDbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options): base(options) {}

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<UserExtention> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(DefaultDataProvider.GetDefaultCategories());
            modelBuilder.Entity<Product>().HasData(DefaultDataProvider.GetDefaultProducts());
            modelBuilder.Entity<Company>().HasData(DefaultDataProvider.GetDefaultCompanies());
        }
    }
}
