using Core.Models;
using Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IOwnersService
    {
        Task<List<OwnersResource?>> GetAll();
        Task<OwnersResource?> GetById(int id);
        Task<OwnersResource?> Add(Owners obj);
        Task<bool> Delete(int id);
        Task<OwnersResource?> Update(int id, Owners obj);
        Task<OwnersResource?> LoginAsync(LoginRequest request);
       
    }
}