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
    public class ImagesRepository : IImagesRepository
    {
        private readonly Context _dbContext;

        public ImagesRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Images?> Add(Images image)
        {
            await _dbContext.Images.AddAsync(image);
            await _dbContext.SaveChangesAsync(); 
            return image;
        }
        public async Task<bool> Delete(int id)
        {
            var q = await _dbContext.Images.FindAsync(id);
            if (q == null)
            {
                return false;
            }
            _dbContext.Remove(q);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Images?>> GetAll()
        {
            return await _dbContext.Images.Include(i => i.Property).ToListAsync<Images?>();
        }
        public async Task<Images?> GetById(int id)
        {
            var q = _dbContext.Images.FindAsync(id);
            return await q;
        }
        public async Task<Images?> Update(int id, Images obj)
        {
            var q = await _dbContext.Images.FindAsync(id);

            if (q == null)
            {
                return null;
            }
            q.AltText = obj.AltText;
            q.CreatedDate = obj.CreatedDate;
            q.ImageUrl = obj.ImageUrl;
            q.PropertyId = obj.PropertyId;

            await _dbContext.SaveChangesAsync();
            return q;
        }
    }
}
