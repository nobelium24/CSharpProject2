using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Database
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        public DbSet<Models.UserModel> Users { get; set; }
        public DbSet<Models.CategoryModel> Categories { get; set; }
        public DbSet<Models.ProductModel> Products { get; set; }
        public DbSet<Models.CartModel> Carts { get; set; }
        public DbSet<Models.ForgotPasswordModel> ForgotPassword { get; set; }
        public DbSet<Models.AdminModel> Admin { get; set; }
        public DbSet<Models.OrderModel> Orders { get; set; }
        public DbSet<Models.ImageModel> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.UserModel>()
                .HasAlternateKey(a => new { a.Email });

            modelBuilder.Entity<Models.UserModel>()
                .HasAlternateKey(u => new {u.StoreName});

            modelBuilder.Entity<Models.CategoryModel>()
                .HasAlternateKey(a => new { a.CategoryName });

            modelBuilder.Entity<Models.AdminModel>()
                .HasAlternateKey(a => new { a.Email });

            modelBuilder.Entity<Models.AdminModel>()
                .HasAlternateKey(a => new { a.UserName });

            modelBuilder.Entity<Models.ImageModel>()
                .HasOne(p => p.Product)
                .WithMany(i => i.Images)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.ProductModel>()
                .HasOne(p => p.User)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.OrderModel>()
                .HasOne(o => o.User)
                .WithMany(u => u.UserOrders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            modelBuilder.Entity<Models.OrderModel>()
                .HasOne(o => o.Seller)
                .WithMany(u => u.SellerOrders)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.OrderModel>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);  

            modelBuilder.Entity<Models.OrderModel>()
                .HasOne(o => o.Seller)
                .WithMany()
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.ProductModel>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.CartModel>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Carts)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.CartModel>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.ProductModel>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.CategoryModel>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.ProductModel>()
                .HasMany(p => p.Carts)
                .WithOne(c => c.Product)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.UserModel>()
                .HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            modelBuilder.Entity<Models.UserModel>()
                .HasMany(u => u.UserOrders)
                .WithOne(o => o.User)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            
            modelBuilder.Entity<Models.UserModel>()
                .HasMany(u => u.SellerOrders)
                .WithOne(o => o.Seller)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Models.ProductModel>()
                .HasMany(p => p.Orders)
                .WithOne(o => o.Product)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}