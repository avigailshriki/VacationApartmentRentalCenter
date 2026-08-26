using AutoMapper;
using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertiesService _propertiesService;
        public PropertiesController(IPropertiesService propertiesService)
        {
            _propertiesService = propertiesService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PropertiesResource>>> GetAll()
        {
            var properties = await _propertiesService.GetAll();
            return Ok(properties);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertiesResource>> GetById(int id)
        {
            var result = await _propertiesService.GetById(id);
            if (result == null) return NotFound("הדירה לא נמצאה");
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<PropertiesResource>> Add([FromBody] Properties resource)
        {
            var result = await _propertiesService.Add(resource);

            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _propertiesService.Delete(id);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertiesResource?>> Update(int id, [FromBody] Properties propertiesResource)
        {
            var result = await _propertiesService.Update(id, propertiesResource);
            if (result == null) return NotFound("הדירה לעדכון לא נמצאה");
            return Ok(result);
        }
        [HttpGet("Search")]
        public async Task<ActionResult<List<PropertiesResource>>> Search(
            [FromQuery] string? title,
            [FromQuery] string? city,
            [FromQuery] double? maxPrice,
            [FromQuery] int? capacity)
        {
            var results = await _propertiesService.GetFiltered(title, city, maxPrice, capacity);
            return Ok(results);
        }
        [HttpGet("MyProperties/{ownerId}")]
        public async Task<ActionResult<List<PropertiesResource>>> GetMyProperties(int ownerId)
        {
            var properties = await _propertiesService.GetOwnerProperties(ownerId);
            if (properties == null || properties.Count == 0)
            {
                return Ok(new List<PropertiesResource>());
            }
            return Ok(properties);
        }
        [HttpPatch("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var updatedProperty = await _propertiesService.ChangeStatus(id);

            if (updatedProperty == null)
            {
                return NotFound($"Property with ID {id} not found.");
            }
            return Ok(updatedProperty);
        }
        [HttpGet("{propertyId}/Reviews")]
        public async Task<ActionResult<List<ReviewResource>>> GetPropertyReviews(int propertyId)
        {
            var reviews = await _propertiesService.GetPropertyReviews(propertyId);
            return Ok(reviews);
        }
        [HttpPost("AddByOwner/{ownerId}")]
        public async Task<IActionResult> AddPropertyByOwnerID(
     int ownerId,
     [FromForm] PropertyAddDto dto,
     List<IFormFile> images) // שינוי ל-List
        {
            // בדיקה אם נשלחו תמונות
            if (images == null || images.Count == 0)
                return BadRequest("חובה לצרף לפחות תמונה אחת.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var property = new Properties
            {
                Title = dto.Title,
                City = dto.City,
                Address = dto.Address,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity,
                Description = dto.Description,
                OwnerID = ownerId,
                Images = new List<Images>() // יצירת רשימה ריקה
            };

            // לולאה לעיבוד כל תמונה ותמונה
            foreach (var image in images)
            {
                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var path = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // הוספת התמונה לרשימת התמונות של הנכס
                property.Images.Add(new Images { ImageUrl = $"/uploads/{fileName}" });
            }

            var result = await _propertiesService.AddPropertyByOwnerID(ownerId, property);
            return Ok(result);
        }
    }
}
