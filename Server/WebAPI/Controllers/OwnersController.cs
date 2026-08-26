using AutoMapper;
using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly ILoginRequestService _loginService;
        private readonly IOwnersService _ownersService;
        private readonly IPropertiesService _propertiesService;
        private readonly EmailService _emailService;
        private readonly ILogger<OwnersController> _logger;

        public OwnersController(
            IOwnersService ownersService,
            ILogger<OwnersController> logger,
            ILoginRequestService loginService,
            EmailService emailService,
            IPropertiesService propertiesService)
        {
            _ownersService = ownersService;
            _logger = logger;
            _loginService = loginService;
            _emailService = emailService;
            _propertiesService = propertiesService; 
        }
        [HttpGet]
        public async Task<List<OwnersResource?>> GetAll()
        {
            return await _ownersService.GetAll();
        }
        [HttpGet("{id}")]
        public async Task<OwnersResource?> GetById(int id)
        {
            return await _ownersService.GetById(id);
        }
        [HttpPost]
        public async Task<ActionResult<OwnersResource>> Add(Owners obj)
        {
            var result = await _ownersService.Add(obj);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Could not add owner.");
        }
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _ownersService.Delete(id);
        }
        [HttpPut("{id}")]
        public async Task<OwnersResource?> Update(int id, Owners obj)
        {
            return await _ownersService.Update(id, obj);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "נא למלא אימייל וסיסמה" });
            }

            var userResource = await _loginService.LoginAsync(request);

            if (userResource != null)
            {
                return Ok(new { Message = "התחברות הצליחה!", User = userResource });
            }

            return Unauthorized(new { Message = "אימייל או סיסמה שגויים, או שאינך רשום במערכת" });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                return BadRequest(new { Message = "נתונים לא תקינים" });

            try
            {
                var result = await _loginService.RegisterAsync(request);
                return Ok(new { Message = "ההרשמה הצליחה!", User = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בהרשמה");
                return BadRequest(new { Message = "מייל זה כבר קיים במערכת או שחלה שגיאה בשרת" });
            }
        }
    }
}