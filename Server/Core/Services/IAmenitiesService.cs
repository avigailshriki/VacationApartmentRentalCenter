using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAmenitiesService
    {
        public Task<List<AmenitiesResource?>> GetAll();
        public Task<AmenitiesResource?> GetById(int id);
        public Task<AmenitiesResource?> Add(Amenities obj);
        public Task<bool> Delete(int id);
        public Task<AmenitiesResource?> Update(int id, Amenities obj);
    }
}
