using AutoMapper;
using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB לכל תמונה

        private readonly IPropertiesService _propertiesService;
        public PropertiesController(IPropertiesService propertiesService)
        {
            _propertiesService = propertiesService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PropertiesResource?>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var properties = await _propertiesService.GetAllPaged(page, pageSize);
            return Ok(properties);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertiesResource>> GetById(int id)
        {
            var result = await _propertiesService.GetById(id);
            if (result == null) return NotFound("הדירה לא נמצאה");
            return Ok(result);
        }

        // יוצר נכס תחת המשתמש המחובר בלבד - ה-OwnerID תמיד נלקח מהטוקן ולא מהבקשה.
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PropertiesResource>> Add([FromBody] Properties resource)
        {
            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null) return Forbid();

            resource.OwnerID = currentOwnerId.Value;
            var result = await _propertiesService.Add(resource);

            return Ok(result);
        }

        // מחיקה מותרת רק לבעלים של הנכס.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var existing = await _propertiesService.GetById(id);
            if (existing == null) return NotFound();

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || existing.OwnerID != currentOwnerId)
                return Forbid();

            var result = await _propertiesService.Delete(id);
            return Ok(result);
        }

        // עדכון מותר רק לבעלים של הנכס. ה-OwnerID תמיד נלקח מהטוקן (לא מהבקשה עצמה),
        // כדי שלא יהיה אפשר "לגנוב" נכס על ידי שליחת OwnerID שונה בבקשת העדכון.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertiesResource?>> Update(int id, [FromBody] Properties propertiesResource)
        {
            var existing = await _propertiesService.GetById(id);
            if (existing == null) return NotFound("הדירה לעדכון לא נמצאה");

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || existing.OwnerID != currentOwnerId)
                return Forbid();

            propertiesResource.OwnerID = currentOwnerId.Value;

            var result = await _propertiesService.Update(id, propertiesResource);
            if (result == null) return NotFound("הדירה לעדכון לא נמצאה");
            return Ok(result);
        }
        [HttpGet("Search")]
        public async Task<ActionResult<PagedResult<PropertiesResource?>>> Search(
            [FromQuery] string? title,
            [FromQuery] string? city,
            [FromQuery] double? maxPrice,
            [FromQuery] int? capacity,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var results = await _propertiesService.GetFilteredPaged(title, city, maxPrice, capacity, page, pageSize);
            return Ok(results);
        }

        // מותר לבקש רק את רשימת הנכסים של המשתמש המחובר עצמו.
        [Authorize]
        [HttpGet("MyProperties/{ownerId}")]
        public async Task<ActionResult<List<PropertiesResource>>> GetMyProperties(int ownerId)
        {
            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || currentOwnerId != ownerId)
                return Forbid();

            var properties = await _propertiesService.GetOwnerProperties(ownerId);
            if (properties == null || properties.Count == 0)
            {
                return Ok(new List<PropertiesResource>());
            }
            return Ok(properties);
        }

        // שינוי סטטוס מותר רק לבעלים של הנכס.
        [Authorize]
        [HttpPatch("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var existing = await _propertiesService.GetById(id);
            if (existing == null)
            {
                return NotFound($"Property with ID {id} not found.");
            }

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || existing.OwnerID != currentOwnerId)
                return Forbid();

            var updatedProperty = await _propertiesService.ChangeStatus(id);

            if (updatedProperty == null)
            {
                return NotFound($"Property with ID {id} not found.");
            }
            return Ok(updatedProperty);
        }
        [HttpGet("Cities")]
        public async Task<ActionResult<List<string>>> GetCities()
        {
            var cities = await _propertiesService.GetDistinctCities();
            return Ok(cities);
        }
        [HttpGet("{propertyId}/Reviews")]
        public async Task<ActionResult<List<ReviewResource>>> GetPropertyReviews(int propertyId)
        {
            var reviews = await _propertiesService.GetPropertyReviews(propertyId);
            return Ok(reviews);
        }

        // הוספת נכס עם תמונות מותרת רק כאשר ownerId בנתיב תואם למשתמש המחובר.
        [Authorize]
        [HttpPost("AddByOwner/{ownerId}")]
        public async Task<IActionResult> AddPropertyByOwnerID(
     int ownerId,
     [FromForm] PropertyAddDto dto,
     List<IFormFile> images) // שינוי ל-List
        {
            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || currentOwnerId != ownerId)
                return Forbid();

            // בדיקה אם נשלחו תמונות
            if (images == null || images.Count == 0)
                return BadRequest("חובה לצרף לפחות תמונה אחת.");

            // ולידציה על כל התמונות (סוג קובץ + גודל) לפני שכותבים משהו לדיסק,
            // וכדי למנוע Path Traversal לא משתמשים בשם הקובץ המקורי בכלל (רק בסיומת שלו).
            foreach (var image in images)
            {
                var extension = Path.GetExtension(image.FileName)?.ToLowerInvariant() ?? string.Empty;
                if (!AllowedImageExtensions.Contains(extension))
                    return BadRequest($"סוג קובץ לא נתמך: {image.FileName}. יש להעלות jpg/jpeg/png/webp בלבד.");

                if (image.Length == 0 || image.Length > MaxImageSizeBytes)
                    return BadRequest($"גודל הקובץ {image.FileName} אינו תקין (מקסימום 10MB לתמונה).");
            }

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
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{extension}";
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
