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
	public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
	{
		public void Configure(EntityTypeBuilder<ChatMessage> builder)
		{
			builder.HasKey(m => m.Id);

			builder.Property(m => m.Text)
				.IsRequired()
				.HasMaxLength(2000);

			builder.Property(m => m.SenderId)
				.IsRequired();

			// Foreign Key Relationship with Sender ApplicationUser
			builder.HasOne<ApplicationUser>()
				.WithMany()
				.HasForeignKey(m => m.SenderId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
