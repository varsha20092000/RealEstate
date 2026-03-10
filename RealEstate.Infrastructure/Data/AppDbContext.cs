using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<VisitBooking> VisitBookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Property → Images
        builder.Entity<Property>()
            .HasMany(p => p.Images)
            .WithOne(i => i.Property)
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Property → Agent (optional)
        builder.Entity<Property>()
            .HasOne(p => p.Agent)
            .WithMany(a => a.Properties)
            .HasForeignKey(p => p.AgentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Agent → User
        builder.Entity<Agent>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<Agent>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Inquiry → Property
        builder.Entity<Inquiry>()
            .HasOne(i => i.Property)
            .WithMany(p => p.Inquiries)
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Favorite → Property
        builder.Entity<Favorite>()
            .HasOne(f => f.Property)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Inquiry>()
    .HasOne(i => i.Agent)
    .WithMany()
    .HasForeignKey(i => i.AgentId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.SetNull);

        // Inquiry → Buyer
        builder.Entity<Inquiry>()
            .HasOne(i => i.Buyer)
            .WithMany()
            .HasForeignKey(i => i.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
        // VisitBooking → Agent (optional)
        builder.Entity<VisitBooking>()
            .HasOne(v => v.Agent)
            .WithMany()
            .HasForeignKey(v => v.AgentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // VisitBooking → Buyer
        builder.Entity<VisitBooking>()
            .HasOne(v => v.Buyer)
            .WithMany()
            .HasForeignKey(v => v.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        // VisitBooking → Property
        builder.Entity<VisitBooking>()
            .HasOne(v => v.Property)
            .WithMany()
            .HasForeignKey(v => v.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        // decimal precision
        builder.Entity<Property>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Entity<Property>()
            .Property(p => p.AreaSqFt)
            .HasPrecision(10, 2);

        builder.Entity<Agent>()
            .Property(a => a.AvgRating)
            .HasPrecision(3, 2);
    }
}