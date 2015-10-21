using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;


namespace TenBackend.Models.Repositories
{
    public interface TURepository
    {
        IEnumerable<TenUser> TenUsers { get; }
    }
}