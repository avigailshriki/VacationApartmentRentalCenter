using Core.Models;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Data.DataRepository
{
    public class PropertiesRepository : IPropertiesReposiory
    {
        private readonly Context _dbContext;

        public PropertiesRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Properties?>> GetAll()
        {
            return await _dbContext.Properties
                .Include(p => p.Amenities)
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .ToListAsync<Properties?>();
        }
        public async Task<Properties?> GetById(int id)
        {
            return await _dbContext.Properties
                .Include(p => p.Amenities)
                .Include(p => p.Owner)
                .Include(p => p.Reviews)
                .Include (p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Properties?> Add(Properties obj)
        {
            
            if (obj.Amenities != null)
            /*{
                foreach (var amenity in obj.Amenities)
                {
                    if (amenity != null && amenity.Id != 0)
                    {
                        _dbContext.Amenities.Attach(amenity);
                    }
                }
            }*/
                        _dbContext.Amenities.AddRange(obj.Amenities);
            //יש לבדוק אם 2 השורות שמעל לא כלולות אוטומטית בשורה הבאה
               await _dbContext.Properties.AddAsync(obj);
                await _dbContext.SaveChangesAsync();
                return obj;
        }
        public async Task<Properties?> AddPropertyByOwnerID(int ownerId, Properties property)
        {
            property.OwnerID = ownerId;
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();
            return property;
        }
        public async Task<bool> Delete(int id)
        {
            
            var property = await _dbContext.Properties
                .Include(p => p.Amenities) 
                .FirstOrDefaultAsync(p => p.Id == id); 

            if (property == null)
            {
                return false; 
            }
            _dbContext.Properties.Remove(property);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<Properties?> Update(int id, Properties obj)
        {
            var existingProperty = await _dbContext.Properties.FindAsync(id);

            if (existingProperty == null) 
                return null;

            existingProperty.OwnerID = obj.OwnerID;
            existingProperty.Title = obj.Title;
            existingProperty.Description = obj.Description;
            existingProperty.PricePerNight = obj.PricePerNight;
            existingProperty.IsAvailable = obj.IsAvailable;
            existingProperty.City = obj.City;
            existingProperty.Amenities = obj.Amenities;
            existingProperty.Capacity = obj.Capacity;
            existingProperty.Address = obj.Address;

            await _dbContext.SaveChangesAsync();
            return existingProperty;
        }
        public async Task<List<Properties?>> GetFiltered(string? title, string? city, double? maxPrice, int? capacity)
        {
            var query = _dbContext.Properties
                .Include(p => p.Amenities)
                .Include(p => p.Images)
              .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(p => p.Title.Contains(title));

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City.Contains(city));

            if (maxPrice.HasValue)
                query = query.Where(p => p.PricePerNight <= maxPrice.Value);

            if (capacity.HasValue)
                query = query.Where(p => p.Capacity >= capacity.Value);

            return await query.ToListAsync<Properties?>();
        }
        public async Task<List<Properties?>> GetPropertiesByOwnerId(int ownerId)
        {
            return await _dbContext.Properties
                .Where(p => p.OwnerID == ownerId).Include(p => p.Images)
                .ToListAsync<Properties?>();
        }
        public async Task<List<Review?>> GetPropertyReviews(int propertyId)
        {
            return await _dbContext.Reviews
               .Include(r => r.Property)
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync<Review?>();
        }
        //public async Task UpdateAsync(Properties property)
        //{
        //    _dbContext.Properties.Update(property);
        //    await _dbContext.SaveChangesAsync();
        //}

    }
}
