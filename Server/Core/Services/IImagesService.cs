using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Services
{
    public interface IImagesService
    {
        public Task<List<ImagesResource?>> GetAll();
        public Task<ImagesResource> Add(Images image);
        public Task<ImagesResource> AddImage(IFormFile file, int propertyId);
        public Task<bool> Delete(int id);
        Task<ImagesResource?> GetById(int id);
    }
}
