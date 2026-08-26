using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface IRepository<T>
    {
        public Task<List<T?>> GetAll();
        public Task<T?> GetById(int id);
        public Task<T?> Add(T obj);
        public Task<T?> Update(int id, T obj);
        public Task<bool> Delete(int id);
    }
}
