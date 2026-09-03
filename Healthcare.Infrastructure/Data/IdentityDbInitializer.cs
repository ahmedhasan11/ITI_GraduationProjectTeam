using Healthcare.Application.Constants;
using Healthcare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Data
{
	public static class IdentityDbInitializer
	{
		public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var services = scope.ServiceProvider;
			var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
			var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
			string[] roles = {
				Permissions.Roles.Admin,
				Permissions.Roles.Doctor,
				Permissions.Roles.Patient,
			};

			foreach (var roleName in roles)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new ApplicationRole(roleName));
				}
			}

			// Seed Default Admin User
			var adminEmail = "admin@mediux.com";
			var admin = await userManager.FindByEmailAsync(adminEmail);
			if (admin == null)
			{
				admin = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					FullName = "System Admin",
					IsPatient = false,
					IsDoctor = false
				};
				var result = await userManager.CreateAsync(admin, "Admin@123");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(admin, "Admin");
				}
			}
		}
	}
}
