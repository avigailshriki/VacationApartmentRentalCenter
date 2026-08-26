using AutoMapper;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ImagesService : IImagesService
    {
        private readonly IImagesRepository _imagesRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImagesService(IImagesRepository imagesRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _imagesRepository = imagesRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<ImagesResource?> GetById(int id)
        {
            var image = await _imagesRepository.GetById(id);
            return _mapper.Map<ImagesResource>(image);
        }
        public async Task<List<ImagesResource?>> GetAll()
        {
            var images = await _imagesRepository.GetAll();
            return _mapper.Map<List<ImagesResource?>>(images);
        }
        public async Task<bool> Delete(int id)
        {
            return await _imagesRepository.Delete(id);
        }
        public async Task<ImagesResource> Add(Images image)
        {
            var addedImage = await _imagesRepository.Add(image);
            return _mapper.Map<ImagesResource>(addedImage);
        }
        public async Task<ImagesResource> AddImage(IFormFile file, int propertyId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("לא נבחר קובץ להעלאה.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("סוג קובץ לא נתמך. נא להעלות תמונה בפורמט jpg או png.");

            var fileName = Guid.NewGuid().ToString() + extension;
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var imageEntity = new Images
            {
                ImageUrl = "/images/" + fileName,
                CreatedDate = DateTime.Now,
                PropertyId = propertyId
            };
            await _imagesRepository.Add(imageEntity);

            return _mapper.Map<ImagesResource>(imageEntity);
        }
    }
}