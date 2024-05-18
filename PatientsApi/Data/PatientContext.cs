using Microsoft.EntityFrameworkCore;
using PatientApi.Models;

namespace PatientApi.Data
{
    public class PatientContext : DbContext
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Name> Names { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Name);

            modelBuilder.Entity<Name>()
                .Property(n => n.Given)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            base.OnModelCreating(modelBuilder);
        }
    }
}
