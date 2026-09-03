using Core.Models;
using Core.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.DataRepository
{
    public class PropertyAvailabilityRepository : IPropertyAvailabilityRepository
    {
        private readonly Context _dbContext;

        public PropertyAvailabilityRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PropertyAvailability?>> GetByPropertyId(int propertyId)
        {
            return await _dbContext.PropertyAvailabilities
                .Where(a => a.PropertyId == propertyId)
                .OrderBy(a => a.StartDate)
                .ToListAsync<PropertyAvailability?>();
        }

        public async Task<PropertyAvailability?> GetById(int id)
        {
            return await _dbContext.PropertyAvailabilities
                .Include(a => a.Property)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<PropertyAvailability?> Add(PropertyAvailability obj)
        {
            await _dbContext.PropertyAvailabilities.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
            return obj;
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _dbContext.PropertyAvailabilities.FindAsync(id);
            if (existing == null)
                return false;

            _dbContext.PropertyAvailabilities.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
