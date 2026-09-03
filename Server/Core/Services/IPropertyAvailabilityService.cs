using Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IPropertyAvailabilityService
    {
        Task<List<PropertyAvailabilityResource?>> GetByPropertyId(int propertyId);
        Task<PropertyAvailabilityResource?> GetById(int id);
        Task<PropertyAvailabilityResource?> Add(PropertyAvailabilityResource obj);
        Task<bool> Delete(int id);
    }
}
