using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using TenBackend.Models.DbContexts;

namespace TenBackend.Models.Repositories
{
    public class TenUserRepository : TURepository {
        private TenUserDbContext dbc = new TenUserDbContext();

        public IEnumerable<TenUser> TenUsers
        {
            get { return dbc.TenUsers; }
        }
    }
}