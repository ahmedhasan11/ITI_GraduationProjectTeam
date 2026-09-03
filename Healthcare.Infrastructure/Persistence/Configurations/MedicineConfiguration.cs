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
	public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
	{
		public void Configure(EntityTypeBuilder<Medicine> builder)
		{
			builder.HasKey(m => m.Id);

			builder.Property(m => m.Name)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(m => m.Description)
				.HasMaxLength(500);

			builder.Property(m => m.Category)
				.HasMaxLength(50);

			// Configure Money Value Object as an Owned Entity Type
			builder.OwnsOne(p => p.Price, money =>
			{
				money.Property(m => m.Amount)
					.HasColumnName("Price")
					.HasPrecision(18, 2)
					.IsRequired();
			});

			builder.Property(m => m.Stock)
				.IsRequired();

			// Soft delete global query filter
			builder.HasQueryFilter(m => !m.IsDeleted);

			builder.HasOne(m => m.Category)
				.WithMany(c => c.Medicines)
				.HasForeignKey(m => m.CategoryId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
