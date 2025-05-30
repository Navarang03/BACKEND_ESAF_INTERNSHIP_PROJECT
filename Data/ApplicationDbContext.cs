using Microsoft.EntityFrameworkCore;
using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<AcademicQualification> AcademicQualifications { get; set; }
        public DbSet<Languages> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Qualifications)
                .WithOne()
                .HasForeignKey(q => q.EmployeeId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.LanguagesKnown)
                .WithOne()
                .HasForeignKey<Languages>(l => l.EmployeeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
