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
	public class CartConfiguration : IEntityTypeConfiguration<Cart>
	{
		public void Configure(EntityTypeBuilder<Cart> builder)
		{
			builder.HasKey(c => c.Id);

			builder.Property(c => c.SessionId)
				.HasMaxLength(100);

			// One Cart has Many CartItems
			builder.HasMany(c => c.Items)
				.WithOne(i => i.Cart)
				.HasForeignKey(i => i.CartId)
				.OnDelete(DeleteBehavior.Cascade);

			// Optional Foreign Key Relationship with ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
