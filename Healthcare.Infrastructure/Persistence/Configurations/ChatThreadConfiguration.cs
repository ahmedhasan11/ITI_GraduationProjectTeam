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
	public class ChatThreadConfiguration : IEntityTypeConfiguration<ChatThread>
	{
		public void Configure(EntityTypeBuilder<ChatThread> builder)
		{
			builder.HasKey(t => t.Id);

			builder.Property(t => t.PatientId)
				.IsRequired();

			builder.Property(t => t.DoctorId)
				.IsRequired();

			builder.HasMany(t => t.Messages)
				.WithOne(m => m.Thread)
				.HasForeignKey(m => m.ThreadId)
				.OnDelete(DeleteBehavior.Cascade);

			// Foreign Key Relationship with Patient ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(t => t.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			// Foreign Key Relationship with Doctor ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(t => t.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
