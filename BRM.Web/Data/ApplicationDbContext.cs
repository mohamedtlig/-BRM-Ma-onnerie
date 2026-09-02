using BRM.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
{
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServicePricing> ServicePricings => Set<ServicePricing>();
    public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();
    public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
    public DbSet<QuotePhoto> QuotePhotos => Set<QuotePhoto>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectPhoto> ProjectPhotos => Set<ProjectPhoto>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Service>()
            .HasOne(s => s.ServicePricing)
            .WithOne(p => p.Service)
            .HasForeignKey<ServicePricing>(p => p.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuoteRequest>()
            .HasOne(q => q.Service)
            .WithMany(s => s.QuoteRequests)
            .HasForeignKey(q => q.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<QuoteRequest>()
            .HasIndex(q => q.NumeroDemande)
            .IsUnique();

        builder.Entity<QuoteItem>()
            .HasOne(i => i.QuoteRequest)
            .WithMany(q => q.QuoteItems)
            .HasForeignKey(i => i.QuoteRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuotePhoto>()
            .HasOne(p => p.QuoteRequest)
            .WithMany(q => q.QuotePhotos)
            .HasForeignKey(p => p.QuoteRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectPhoto>()
            .HasOne(p => p.Project)
            .WithMany(p => p.ProjectPhotos)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
