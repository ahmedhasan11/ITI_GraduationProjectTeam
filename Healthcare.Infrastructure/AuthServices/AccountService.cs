using Healthcare.Application.Models.Account;
using Healthcare.Application.ServiceContracts;
using Healthcare.Domain.Entities;
using Healthcare.Infrastructure.Data;
using Healthcare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.AuthServices
{
	public class AccountService : IAccountService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _db;
		private readonly IOrderService _orderservice;
		public AccountService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			RoleManager<IdentityRole> roleManager,
			ApplicationDbContext db,
			IOrderService orderservice)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_db = db;
			_orderservice = orderservice;
		}

		public async Task Login(LoginViewModel model)
		{
			throw new NotImplementedException();
		}

		public async Task Register(RegisterViewModel model)
		{
			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FullName = model.FullName,
				IsDoctor = model.Role == "Doctor",
				IsPatient = model.Role == "Patient"
			};
		}
	}
}
