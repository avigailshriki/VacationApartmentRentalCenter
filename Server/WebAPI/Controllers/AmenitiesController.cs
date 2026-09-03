using AutoMapper;
using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenitiesController: ControllerBase
    {

        private readonly IAmenitiesService _amenitiesService;
        private readonly ILogger<AmenitiesController> _logger;

        public AmenitiesController(IAmenitiesService amenitiesService, ILogger<AmenitiesController> logger)
        {
            _amenitiesService = amenitiesService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<List<AmenitiesResource?>> GetAll()
        {
            var p = await _amenitiesService.GetAll();
            return p;
        }
        [HttpGet("{id}")]
        public async Task<AmenitiesResource?> GetById(int id)
        {
            var result = await _amenitiesService.GetById(id);
            return result;
        }

        [Authorize]
        [HttpPost]
        public async Task<AmenitiesResource?> Add(Amenities amenities)
        {
            var result = await _amenitiesService.Add(amenities);
            return result;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            var result = await _amenitiesService.Delete(id);
            return result;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<AmenitiesResource?> Update(int id, Amenities amenities)
        {
            var result = await _amenitiesService.Update(id, amenities);
            return result;
        }
       
    }
}
