using Core.Models;
using Core.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataRepository
{
    public class AmenitiesRepository : IAmenitiesRepository
    {
        private readonly Context _dbContext;
        
        public AmenitiesRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Amenities?> Add(Amenities obj)
        {
            var addedAmenity =  _dbContext.Amenities.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
            return obj;
        }
        public async Task<bool> Delete(int id)
        {
            var Amenity = await _dbContext.Amenities.FindAsync(id);
            if (Amenity == null)
            {
                return false;
            }
            _dbContext.Amenities.Remove(Amenity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Amenities?>> GetAll()
        {
            return await _dbContext.Amenities.ToListAsync<Amenities?>();
        }
        public async Task<Amenities?> GetById(int id)
        {
            return await _dbContext.Amenities.FindAsync(id);
        }
        public async Task<Amenities?> Update(int id, Amenities obj)
        {
            var existingAmenity = await _dbContext.Amenities.FindAsync(id);

            if (existingAmenity == null)
            {
                return null;
            }
            existingAmenity.Price = obj.Price;
            existingAmenity.Name = obj.Name;

            _dbContext.Entry(existingAmenity).CurrentValues.SetValues(obj);

            await _dbContext.SaveChangesAsync();
            return existingAmenity;
        }
    }
}
       