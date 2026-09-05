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
	public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
	{
		public void Configure(EntityTypeBuilder<CartItem> builder)
		{
			builder.HasKey(c => c.Id);

			builder.Property(c => c.Quantity)
				.IsRequired();

			builder.HasOne(c => c.Medicine)
				.WithMany()
				.HasForeignKey(c => c.MedicineId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
