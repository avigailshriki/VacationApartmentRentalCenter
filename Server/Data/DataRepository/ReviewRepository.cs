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
    public class ReviewRepository : IReviewRepository
    {
        private readonly Context _dbContext;
        public ReviewRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Review?> Add(Review obj)
        {
            await _dbContext.Reviews.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
            return obj;
        }
        public async Task<bool> Delete(int id)
        {
            var q = await _dbContext.Reviews.FindAsync(id);
            if (q == null) 
                return false;

            _dbContext.Reviews.Remove(q);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Review?>> GetAll()
        {
            return await _dbContext.Reviews.ToListAsync<Review?>();
        }
        public async Task<Review?> GetById(int id)
        {
            return await _dbContext.Reviews.FindAsync(id);
        }
        public async Task<List<Review?>> GetReviewsByPropertyId(int propertyId)
        {
            return await _dbContext.Reviews.Where(r => r.PropertyId == propertyId).ToListAsync<Review?>();
        }
        public async Task<Review?> Update(int id, Review obj)
        {
            var existingReview = await _dbContext.Reviews.FindAsync(id);
            if (existingReview != null)
            {
                existingReview.Rating = obj.Rating;
                existingReview.Comment = obj.Comment;
                existingReview.Name = obj.Name;
                existingReview.Date = obj.Date;

                await _dbContext.SaveChangesAsync();
                return existingReview;
            }
            return null;
        }
    }
}
