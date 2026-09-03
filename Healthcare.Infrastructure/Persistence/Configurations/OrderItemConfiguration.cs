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
	public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.HasKey(i => i.Id);

			builder.Property(i=>i.Quantity).IsRequired();

			builder.Property(i => i.UnitPrice)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.HasOne(i => i.Medicine)
				.WithMany()
				.HasForeignKey(i => i.MedicineId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
