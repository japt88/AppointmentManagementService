using AppointmentManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementService.Data
{
    public class AppointmentDbContext : DbContext
    {

        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().HasKey(a => a.Id);
            modelBuilder.Entity<Patient>().HasKey(a => a.Id);
        }

    }
}
