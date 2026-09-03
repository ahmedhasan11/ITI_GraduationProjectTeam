using Healthcare.Domain.RepositoryInterfaces;
using Healthcare.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Persistence.Repositories
{
	public class ConsultationRepository : IConsultationRepository
	{
		private readonly ApplicationDbContext _db;
		public ConsultationRepository(ApplicationDbContext db)
		{
			_db = db;
		}
	}
}
