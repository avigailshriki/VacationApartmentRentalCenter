using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController: ControllerBase
    {
        private readonly IImagesService _imagesService;
        private readonly IPropertiesService _propertiesService;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IImagesService imagesService, IPropertiesService propertiesService, ILogger<ImagesController> logger)
        {
            _imagesService = imagesService;
            _propertiesService = propertiesService;
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

        // מחיקת תמונה מותרת רק לבעלים של הנכס שאליו היא שייכת.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var image = await _imagesService.GetById(id);
            if (image == null) return NotFound();

            var currentOwnerId = User.GetOwnerId();
            if (image.Property == null || currentOwnerId == null || image.Property.OwnerID != currentOwnerId)
                return Forbid();

            var result = await _imagesService.Delete(id);
            return Ok(result);
        }

        // הוספת תמונה מותרת רק לבעלים של הנכס שאליו היא משויכת.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(Images image)
        {
            var property = await _propertiesService.GetById(image.PropertyId);
            if (property == null) return NotFound("הנכס לא נמצא.");

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || property.OwnerID != currentOwnerId)
                return Forbid();

            var result = await _imagesService.Add(image);
            return Ok(result);
        }

        // העלאת תמונה מותרת רק לבעלים של הנכס.
        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] int propertyId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a file.");

            var property = await _propertiesService.GetById(propertyId);
            if (property == null) return NotFound("הנכס לא נמצא.");

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || property.OwnerID != currentOwnerId)
                return Forbid();

            try
            {
                var result = await _imagesService.AddImage(file, propertyId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
