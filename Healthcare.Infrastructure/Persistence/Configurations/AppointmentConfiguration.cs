using Healthcare.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Persistence.Configurations
{
	public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
	{
		public void Configure(EntityTypeBuilder<Appointment> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.DoctorId)
				.IsRequired();

			builder.Property(a => a.AppointmentDate)
				.IsRequired();

			builder.Property(a => a.StartTime)
				.IsRequired();

			builder.Property(a => a.EndTime)
				.IsRequired();


			// Foreign Key Relationship with Doctor ApplicationUser (Restrict Delete)
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(a => a.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			// Foreign Key Relationship with Patient ApplicationUser (SetNull Delete)
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(a => a.PatientId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
