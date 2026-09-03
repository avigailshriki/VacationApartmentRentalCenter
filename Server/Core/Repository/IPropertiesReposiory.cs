using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface IPropertiesReposiory: IRepository<Properties>
    {
        public Task<List<Properties?>> GetFiltered(string? title, string? city, double? maxPrice, int? capacity);
        public Task<List<Properties?>> GetPropertiesByOwnerId(int ownerId);
        public Task<List<Review?>> GetPropertyReviews(int propertyId);
        public Task<Properties?> AddPropertyByOwnerID(int ownerId, Properties property);
        public Task<(List<Properties?> Items, int TotalCount)> GetAllPaged(int page, int pageSize);
        public Task<(List<Properties?> Items, int TotalCount)> GetFilteredPaged(string? title, string? city, double? maxPrice, int? capacity, int page, int pageSize);
        public Task<List<string>> GetDistinctCities();
        //public Task UpdateAsync(Properties property); 
    }
}
