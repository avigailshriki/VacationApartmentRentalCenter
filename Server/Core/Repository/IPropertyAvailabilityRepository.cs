using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface IPropertyAvailabilityRepository
    {
        Task<List<PropertyAvailability?>> GetByPropertyId(int propertyId);
        Task<PropertyAvailability?> GetById(int id);
        Task<PropertyAvailability?> Add(PropertyAvailability obj);
        Task<bool> Delete(int id);
    }
}
