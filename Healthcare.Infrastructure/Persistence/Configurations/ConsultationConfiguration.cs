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
	public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
	{
		public void Configure(EntityTypeBuilder<Consultation> builder)
		{
			builder.HasKey(c => c.Id);

			builder.Property(c => c.PatientId)
				.IsRequired();

			builder.Property(c => c.DoctorId)
				.IsRequired();

			builder.Property(c => c.AmountPaid)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			// Foreign Key Relationship with Patient ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(c => c.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			// Foreign Key Relationship with Doctor ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(c => c.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
