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
	public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
	{
		public void Configure(EntityTypeBuilder<PatientProfile> builder)
		{
			builder.HasKey(p => p.Id);

			builder.Property(p => p.UserId)
				.IsRequired();

			builder.Property(p => p.Gender)
				.HasMaxLength(10);

			builder.Property(p => p.Address)
				.HasMaxLength(250);

			// Foreign Key Relationship with ApplicationUser (Cascade Delete)
			builder.HasOne<ApplicationUser>()
				.WithOne()
				.HasForeignKey<PatientProfile>(p => p.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
