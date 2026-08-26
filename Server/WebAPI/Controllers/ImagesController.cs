using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController: ControllerBase
    {
        private readonly IImagesService _imagesService;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IImagesService imagesService, ILogger<ImagesController> logger)
        {
            _imagesService = imagesService;
            _logger = logger;
        }
        [HttpGet]
        public Task<List<ImagesResource?>> GetAll()
        {
            return _imagesService.GetAll();
        }
        [HttpGet("{id}")]
        public Task<ImagesResource?> GetById(int id)
        {
            return _imagesService.GetById(id);
        }
        [HttpDelete]
        public Task Delete(int id)
        {
            return _imagesService.Delete(id);
        }
        [HttpPost]
        public async Task<ImagesResource> Add(Images image)
        {
            return await _imagesService.Add(image);
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int propertyId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a file.");

            var result = await _imagesService.AddImage(file, propertyId);
            return Ok(result);
        }
    }
}
