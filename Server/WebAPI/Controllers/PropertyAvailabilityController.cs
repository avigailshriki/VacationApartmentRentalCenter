using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyAvailabilityController : ControllerBase
    {
        private readonly IPropertyAvailabilityService _availabilityService;
        private readonly IPropertiesService _propertiesService;

        public PropertyAvailabilityController(IPropertyAvailabilityService availabilityService, IPropertiesService propertiesService)
        {
            _availabilityService = availabilityService;
            _propertiesService = propertiesService;
        }

        // ציבורי - כל מבקר בעמוד הנכס צריך לראות אילו תאריכים תפוסים.
        [HttpGet("Property/{propertyId}")]
        public async Task<ActionResult<List<PropertyAvailabilityResource>>> GetByPropertyId(int propertyId)
        {
            var result = await _availabilityService.GetByPropertyId(propertyId);
            return Ok(result);
        }

        // חסימת תאריכים מותרת רק לבעל הנכס.
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PropertyAvailabilityResource>> Add([FromBody] PropertyAvailabilityResource resource)
        {
            var property = await _propertiesService.GetById(resource.PropertyId);
            if (property == null) return NotFound("הנכס לא נמצא.");

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || property.OwnerID != currentOwnerId)
                return Forbid();

            try
            {
                var result = await _availabilityService.Add(resource);
                if (result == null) return BadRequest("שגיאה בחסימת התאריכים.");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ביטול חסימה מותר רק לבעל הנכס.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var existing = await _availabilityService.GetById(id);
            if (existing == null) return NotFound();

            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || existing.Property == null || existing.Property.OwnerID != currentOwnerId)
                return Forbid();

            var result = await _availabilityService.Delete(id);
            return Ok(result);
        }
    }
}
