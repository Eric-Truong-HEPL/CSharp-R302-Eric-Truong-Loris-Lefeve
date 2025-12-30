using KindomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
//sépare ApplicationDbContext des entités   
namespace KindomHospital.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Ordonnance> Ordonnances { get; set; }
        public DbSet<OrdonnanceLigne> OrdonnanceLignes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes & unique constraints
            modelBuilder.Entity<Specialty>().HasIndex(s => s.Name).IsUnique();
            // Ensure medicament name uniqueness as required by the specification
            modelBuilder.Entity<Medicament>().HasIndex(m => m.Name).IsUnique();

            // Specialty -> Doctors (1->N) : protect delete
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor -> Consultations (1->N) : restrict delete (historisation)
            modelBuilder.Entity<Consultation>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Consultations)
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient -> Consultations (1->N)
            modelBuilder.Entity<Consultation>()
                .HasOne(c => c.Patient)
                .WithMany(p => p.Consultations)
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ordonnance -> Lignes : cascade delete (suppression d'une ordonnance supprime ses lignes)
            modelBuilder.Entity<OrdonnanceLigne>()
                .HasOne(l => l.Ordonnance)
                .WithMany(o => o.Lignes)
                .HasForeignKey(l => l.OrdonnanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ordonnance -> Doctor / Patient : restrict
            modelBuilder.Entity<Ordonnance>()
                .HasOne(o => o.Doctor)
                .WithMany(d => d.Ordonnances)
                .HasForeignKey(o => o.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ordonnance>()
                .HasOne(o => o.Patient)
                .WithMany(p => p.Ordonnances)
                .HasForeignKey(o => o.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add additional constraints (ex: index on consultation for scheduling)
            modelBuilder.Entity<Consultation>()
                .HasIndex(c => new { c.DoctorId, c.Date, c.Hour })
                .HasDatabaseName("IX_Consultation_Schedule");
        }
    }
}
