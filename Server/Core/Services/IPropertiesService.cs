using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IPropertiesService
    {
        public Task<List<PropertiesResource?>> GetAll();
        public Task<PropertiesResource?> GetById(int id);
        public Task<PropertiesResource?> Add(Properties properties);
        public Task<bool> Delete(int id);
        public Task<PropertiesResource?> Update(int id, Properties p);
        public Task<List<PropertiesResource>> GetFiltered(string? title, string? city, double? maxPrice, int? capacity);
        public Task<List<PropertiesResource>> GetOwnerProperties(int ownerId);
        public Task<List<Review?>> GetPropertyReviews(int propertyId);
        public Task<PropertiesResource?> ChangeStatus(int id);
        Task<PropertiesResource> AddPropertyByOwnerID(int ownerId, Properties property);
    }
}
