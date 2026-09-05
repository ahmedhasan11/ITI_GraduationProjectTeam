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
				.HasConversion<string>()
				.IsRequired()
				.HasMaxLength(20);

			builder.Property(o => o.RequiresRefund)
				.IsRequired();

			builder.Property(o => o.SubTotal)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.Property(o => o.ShippingFee)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.Property(o => o.TotalAmount)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.OwnsOne(o => o.Address, sa =>
			{
				sa.Property(a => a.RecipientName).HasMaxLength(150).IsRequired();
				sa.Property(a => a.PhoneNumber).HasMaxLength(30).IsRequired();
				sa.Property(a => a.City).HasMaxLength(100).IsRequired();
				sa.Property(a => a.Street).HasMaxLength(200).IsRequired();
				sa.Property(a => a.BuildingNumber).HasMaxLength(50);
				sa.Property(a => a.PostalCode).HasMaxLength(20);
			});

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
