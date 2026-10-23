using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(e =>
        {
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.StudentNo).HasMaxLength(20);
            e.Property(u => u.ProfilePhotoUrl).HasMaxLength(300);
        });

        builder.Entity<Category>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(50).IsRequired();
            e.Property(c => c.Icon).HasMaxLength(50);
            e.HasIndex(c => c.Name).IsUnique();
        });

        builder.Entity<Location>(e =>
        {
            e.Property(l => l.Building).HasMaxLength(100).IsRequired();
            e.Property(l => l.FloorOrRoom).HasMaxLength(100);
        });

        builder.Entity<Item>(e =>
        {
            e.Property(i => i.Title).HasMaxLength(100).IsRequired();
            e.Property(i => i.Description).HasMaxLength(2000).IsRequired();
            e.Property(i => i.VerificationQuestion).HasMaxLength(300);
            e.Property(i => i.Type).HasConversion<string>().HasMaxLength(20);
            e.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

            e.HasOne(i => i.Category).WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.Location).WithMany(l => l.Items)
                .HasForeignKey(i => i.LocationId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.Owner).WithMany(u => u.Items)
                .HasForeignKey(i => i.OwnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.ApprovedBy).WithMany()
                .HasForeignKey(i => i.ApprovedById).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(i => new { i.Status, i.Type, i.EventDate });
        });

        SeedData.Apply(builder);
    }
}
