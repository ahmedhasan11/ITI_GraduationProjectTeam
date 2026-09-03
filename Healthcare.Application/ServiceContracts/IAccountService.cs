using Healthcare.Application.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Application.ServiceContracts
{
	public interface IAccountService
	{
		Task Register(RegisterViewModel model);
		Task Login(LoginViewModel model);
	}
}
