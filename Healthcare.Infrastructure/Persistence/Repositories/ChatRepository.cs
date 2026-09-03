using Healthcare.Domain.RepositoryInterfaces;
using Healthcare.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Persistence.Repositories
{
	public class ChatRepository : IChatRepository
	{
		private readonly ApplicationDbContext _db;
		public ChatRepository(ApplicationDbContext db)
		{
			_db = db;
		}
	}
}
