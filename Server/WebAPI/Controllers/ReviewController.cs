using AutoMapper;
using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ReviewController : ControllerBase 
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ReviewResource>>> GetAll()
        {
            var reviews = await _reviewService.GetAll();
            return Ok(reviews);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResource>> GetById(int id)
        {
            var review = await _reviewService.GetById(id);
            if (review == null) return NotFound("ביקורת לא נמצאה");
            return Ok(review);
        }
        [HttpPost]
        public async Task<ActionResult<ReviewResource>> Add([FromBody] ReviewResource reviewResource)
        {
            var result = await _reviewService.Add(reviewResource);
            if (result == null) return BadRequest("שגיאה בהוספת הביקורת");
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _reviewService.Delete(id);
            if (!result) return NotFound("הביקורת למחיקה לא נמצאה");
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewResource>> Update(int id, [FromBody] ReviewResource reviewResource)
        {
            var result = await _reviewService.Update(id, reviewResource);
            if (result == null) return NotFound("הביקורת לעדכון לא נמצאה");
            return Ok(result);
        }
    }
}