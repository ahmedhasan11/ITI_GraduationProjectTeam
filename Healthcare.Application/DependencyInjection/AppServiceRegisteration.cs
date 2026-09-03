using Healthcare.Application.ServiceContracts;
using Healthcare.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Application.DependencyInjection
{
	public static class AppServiceRegisteration
	{
		public static IServiceCollection AppServices(this IServiceCollection services)
		{
			services.AddScoped<IDoctorService, DoctorService>();
			services.AddScoped<IMedicineService, MedicineService>();
			services.AddScoped<ICartService, CartService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IConsultationService, ConsultationService>();
			services.AddScoped<IAppointmentService, AppointmentService>();
			services.AddScoped<IChatService, ChatService>();

			return services;
		}
	}
}
