using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface IOwnersRepository : IRepository<Owners>
    {
        public Task<Owners?> GetByEmail(string email);
        public Task<Owners?> GetByEmailAndPassword(string email, string password);
    }
}
