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

        }
    }
}