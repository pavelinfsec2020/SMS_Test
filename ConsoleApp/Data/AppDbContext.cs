using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<MenuItemEntity> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItemEntity>()
                .HasIndex(m => m.Article)
                .IsUnique()
                .HasDatabaseName("ix_menu_items_article");
        }
    }
}
