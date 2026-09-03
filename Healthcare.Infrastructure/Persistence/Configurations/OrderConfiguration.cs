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
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.HasKey(o => o.Id);

			builder.Property(o => o.PatientId)
				.IsRequired();

			builder.Property(o => o.Status)
				.IsRequired()
				.HasMaxLength(20);

			builder.Property(o => o.Total)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.HasMany(o => o.Items)
				.WithOne(i => i.Order)
				.HasForeignKey(i => i.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			// Foreign Key Relationship with ApplicationUser (Restrict Delete)
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(o => o.PatientId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
