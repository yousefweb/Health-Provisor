using _5Dots.Models;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthProvisor.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public DbSet<Patient> Patients { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
		public DbSet<Visa> Visas { get; set; }
		public DbSet<Testimonial> Testimonials { get; set; }
		public DbSet<Review> Reviews { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Doctor>()
					   .HasOne(d => d.User)
					   .WithOne(u => u.Doctor)
					   .HasForeignKey<Doctor>(d => d.UserId);

			modelBuilder.Entity<Patient>()
				.HasOne(p => p.User)
				.WithOne(u => u.Patient)
				.HasForeignKey<Patient>(p => p.UserId);

			modelBuilder.Entity<Consultation>()
				.HasOne(c => c.Patient)
				.WithMany(p => p.Consultations)
				.HasForeignKey(c => c.PatientID);

			modelBuilder.Entity<Consultation>()
				.HasOne(c => c.Doctor)
				.WithMany(d => d.Consultations)
				.HasForeignKey(c => c.DoctorID);
		}
	}
}
