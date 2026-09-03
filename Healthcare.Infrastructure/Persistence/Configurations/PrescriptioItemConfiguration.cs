using Healthcare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Persistence.Configurations
{
	public class PrescriptioItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
	{
		public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
		{
			builder.HasKey(i => i.Id);

			builder.Property(i => i.Notes)
				.HasMaxLength(500);

			builder.HasOne(i => i.Medicine)
				.WithMany()
				.HasForeignKey(i => i.MedicineId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
