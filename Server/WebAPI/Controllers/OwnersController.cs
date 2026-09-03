using AutoMapper;
using Core.Exceptions;
using Core.Models;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Extensions;

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
        private readonly ITokenService _tokenService;
        private readonly ILogger<OwnersController> _logger;

        public OwnersController(
            IOwnersService ownersService,
            ILogger<OwnersController> logger,
            ILoginRequestService loginService,
            EmailService emailService,
            IPropertiesService propertiesService,
            ITokenService tokenService)
        {
            _ownersService = ownersService;
            _logger = logger;
            _loginService = loginService;
            _emailService = emailService;
            _propertiesService = propertiesService;
            _tokenService = tokenService;
        }

        // דורש התחברות - מונע רשימה ציבורית של כל המשתמשים במערכת.
        [Authorize]
        [HttpGet]
        public async Task<List<OwnersResource?>> GetAll()
        {
            return await _ownersService.GetAll();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<OwnersResource?> GetById(int id)
        {
            return await _ownersService.GetById(id);
        }

        // מותר למחוק רק את המשתמש המחובר עצמו.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || currentOwnerId != id)
                return Forbid();

            var result = await _ownersService.Delete(id);
            return Ok(result);
        }

        // מותר לעדכן רק את המשתמש המחובר עצמו.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Owners obj)
        {
            var currentOwnerId = User.GetOwnerId();
            if (currentOwnerId == null || currentOwnerId != id)
                return Forbid();

            var result = await _ownersService.Update(id, obj);
            if (result == null) return NotFound();
            return Ok(result);
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
                var token = _tokenService.GenerateToken(userResource.Id, userResource.Email, userResource.FullName);
                return Ok(new { Message = "התחברות הצליחה!", User = userResource, Token = token });
            }

            return Unauthorized(new { Message = "אימייל או סיסמה שגויים, או שאינך רשום במערכת" });
        }

        // התחברות/הרשמה אוטומטית דרך גוגל: הקליינט שולח את ה-ID Token שגוגל הנפיקה בדפדפן,
        // השרת מאמת אותו מול גוגל ומחזיר בדיוק את אותה תגובה כמו התחברות רגילה (Message/User/Token),
        // כדי שהקליינט יוכל להשתמש באותו קוד טיפול בתגובה.
        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var userResource = await _loginService.GoogleLoginAsync(request.IdToken);

            if (userResource != null)
            {
                var token = _tokenService.GenerateToken(userResource.Id, userResource.Email, userResource.FullName);
                return Ok(new { Message = "התחברות עם גוגל הצליחה!", User = userResource, Token = token });
            }

            return Unauthorized(new { Message = "אימות מול גוגל נכשל" });
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
            catch (DuplicateEmailException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בהרשמה");
                return StatusCode(500, new { Message = "אירעה שגיאה בשרת בעת ההרשמה, נסה שוב מאוחר יותר" });
            }
        }
    }
}
