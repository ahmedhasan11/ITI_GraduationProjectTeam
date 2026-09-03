using Healthcare.Domain.Entities;
using Healthcare.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Persistence.Configurations
{
	public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
	{
		public void Configure(EntityTypeBuilder<Prescription> builder)
		{
			builder.HasKey(p => p.Id);

			builder.Property(p => p.DoctorId)
				.IsRequired();

			builder.Property(p => p.PatientId)
				.IsRequired();

			builder.HasMany(p => p.Items)
				.WithOne(i => i.Prescription)
				.HasForeignKey(i => i.PrescriptionId)
				.OnDelete(DeleteBehavior.Cascade);

			// Foreign Key Relationship with Doctor ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(p => p.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			// Foreign Key Relationship with Patient ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(p => p.PatientId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
