using Healthcare.Domain.RepositoryInterfaces;
using Healthcare.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Infrastructure.Persistence.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly ApplicationDbContext _db;
		public CartRepository(ApplicationDbContext db)
		{
			_db = db;
		}
	}
}
