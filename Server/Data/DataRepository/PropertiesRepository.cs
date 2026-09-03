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
            // הערה: IsAvailable כבר לא מנוהל דרך טופס העריכה (הוחלף בלוח הזמינות),
            // ולכן לא נוגעים בו כאן - אחרת הוא היה מתאפס ל-false בכל שמירה כי הטופס לא שולח אותו.
            existingProperty.City = obj.City;
            // הערה: לא מעדכנים כאן Amenities/Reviews/Images - טופס העריכה לא שולח אותם,
            // ועדכון שלהם כאן היה מוחק אותם בטעות (undefined/null) בכל שמירה.
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
        public async Task<(List<Properties?> Items, int TotalCount)> GetAllPaged(int page, int pageSize)
        {
            var query = _dbContext.Properties
                .Include(p => p.Amenities)
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .OrderBy(p => p.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync<Properties?>();

            return (items, totalCount);
        }
        public async Task<(List<Properties?> Items, int TotalCount)> GetFilteredPaged(string? title, string? city, double? maxPrice, int? capacity, int page, int pageSize)
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

            query = query.OrderBy(p => p.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync<Properties?>();

            return (items, totalCount);
        }
        public async Task<List<string>> GetDistinctCities()
        {
            return await _dbContext.Properties
                .Where(p => p.City != null && p.City != "")
                .Select(p => p.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
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
