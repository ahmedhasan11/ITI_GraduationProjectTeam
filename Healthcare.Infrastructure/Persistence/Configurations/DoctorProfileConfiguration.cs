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
	public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
	{
		public void Configure(EntityTypeBuilder<DoctorProfile> builder)
		{
			builder.HasKey(d => d.Id);

			builder.Property(d => d.UserId)
				.IsRequired();

			builder.Property(d => d.Specialty)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(d => d.Bio)
				.HasMaxLength(1000);

			builder.Property(d => d.LicenseNumber)
				.HasMaxLength(50);

			// Foreign Key Relationship with ApplicationUser (Cascade Delete)
			builder.HasOne<ApplicationUser>()
				.WithOne()
				.HasForeignKey<DoctorProfile>(d => d.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
