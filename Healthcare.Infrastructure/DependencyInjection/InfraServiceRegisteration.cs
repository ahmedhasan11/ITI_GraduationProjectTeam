

using Healthcare.Application.ServiceContracts;
using Healthcare.Infrastructure.Data;
using Healthcare.Infrastructure.Identity;
using Healthcare.Infrastructure.Stripe;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Healthcare.Infrastructure.DependencyInjection
{
	public static class InfraServiceRegisteration
	{
		public static IServiceCollection InfraServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Register DbContext with SQL Server
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
			
			// Register Identity (UserManager, RoleManager, SignInManager, etc.) with ApplicationUser & ApplicationRole (Guid)
			services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

			// Register Stripe Configuration Options
			services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

			services.AddScoped<IAccountService, Healthcare.Infrastructure.AuthServices.AccountService>();
			return services;
		}
	}
}
