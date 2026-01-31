using BookTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookTracking.Infrastructure.Data;

public class BookTrackingDbContext : DbContext
{
    public BookTrackingDbContext(DbContextOptions<BookTrackingDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- 1. BOOK CONFIGURATION ---
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200); 
            entity.Property(e => e.Description)
                .IsRequired(false)
                .HasMaxLength(2000);
            entity.Property(e => e.PublishDate)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // --- 2. AUTHOR CONFIGURATION ---
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        //--- 3. RELATIONSHIP CONFIGURATION ---
        // Many-to-Many Relationship between Book and Author
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany(a => a.Books)
            .UsingEntity(j => j.ToTable("BookAuthors"));

        // --- 4. AUDIT LOG CONFIGURATION ---
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Action)
                .IsRequired();
            entity.Property(e => e.EntityType)
                .IsRequired();
            entity.Property(e => e.EntityId)
                .IsRequired();
            entity.Property(e => e.PropertyName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.OldValue)
                .IsRequired(false)
                .HasMaxLength(4000);
            entity.Property(e => e.NewValue)
                .IsRequired(false)
                .HasMaxLength(4000);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        base.OnModelCreating(modelBuilder);
    }
}