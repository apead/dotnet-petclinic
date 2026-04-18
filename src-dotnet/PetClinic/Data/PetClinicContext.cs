using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data;

public class PetClinicContext(DbContextOptions<PetClinicContext> options) : DbContext(options)
{
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<PetType> PetTypes => Set<PetType>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Vet> Vets => Set<Vet>();
    public DbSet<Specialty> Specialties => Set<Specialty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(b =>
        {
            b.ToTable("owners");
            b.HasKey(o => o.Id);
            b.HasMany(o => o.Pets)
             .WithOne()
             .HasForeignKey(p => p.OwnerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Pet>(b =>
        {
            b.ToTable("pets");
            b.HasKey(p => p.Id);
            b.HasOne(p => p.Type)
             .WithMany()
             .HasForeignKey(p => p.TypeId);
            b.HasMany(p => p.Visits)
             .WithOne()
             .HasForeignKey(v => v.PetId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PetType>(b => b.ToTable("types"));
        modelBuilder.Entity<Visit>(b =>
        {
            b.ToTable("visits");
            b.HasOne(v => v.Vet)
             .WithMany()
             .HasForeignKey(v => v.VetId)
             .IsRequired(false);
        });
        modelBuilder.Entity<Specialty>(b => b.ToTable("specialties"));

        modelBuilder.Entity<Vet>(b =>
        {
            b.ToTable("vets");
            b.HasMany(v => v.Specialties)
             .WithMany()
             .UsingEntity(j => j.ToTable("vet_specialties"));
        });
    }
}
