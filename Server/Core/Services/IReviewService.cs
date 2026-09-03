using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IReviewService
    {
        public Task<List<ReviewResource?>> GetAll();
        public Task<ReviewResource?> GetById(int id);
        public Task<ReviewResource?> Add(ReviewResource obj);
        public Task<bool> Delete(int id);
        public Task<ReviewResource?> Update(int id, ReviewResource obj);
        //public Task<List<ReviewResource?>> GetReviewsByPropertyId(int propertyId);

    }
}
