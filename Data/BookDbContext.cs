using BookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data;

// Cấu hình viết theo schema SQL trong Database/01_Create_BookManagementDB.sql.
public partial class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Author).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Genre).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Publisher).HasMaxLength(150);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Description).HasMaxLength(2000);
        });
    }
}
