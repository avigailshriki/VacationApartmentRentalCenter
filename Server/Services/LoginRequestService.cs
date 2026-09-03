using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Exceptions;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class LoginRequestService : ILoginRequestService
    {
        private readonly IOwnersRepository _ownersRepository;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginRequestService> _logger;
        private readonly IConfiguration _configuration;

        public LoginRequestService(
            IOwnersRepository ownersRepository,
            EmailService emailService,
            IMapper mapper,
            ILogger<LoginRequestService> logger,
            IConfiguration configuration)
        {
            _ownersRepository = ownersRepository;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OwnersResource?> LoginAsync(LoginRequest request)
        {
            var owner = await _ownersRepository.GetByEmail(request.Email);
            if (owner == null || string.IsNullOrEmpty(owner.Password))
                return null;

            if (!IsPasswordValid(request.Password, owner.Password))
                return null;

            var resource = _mapper.Map<OwnersResource>(owner);
            return resource;
        }
        public async Task<OwnersResource?> RegisterAsync(RegisterRequest request)
        {
            var existingOwner = await _ownersRepository.GetByEmail(request.Email);
            if (existingOwner != null)
            {
                throw new DuplicateEmailException("משתמש עם אימייל זה כבר קיים במערכת.");
            }
            var names = request.FullName.Split(' ', 2);
            string firstName = names[0];
            string lastName = names.Length > 1 ? names[1] : "";

            var newOwner = new Owners
            {
                FirstName = firstName,
                LastName = lastName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            var registeredUser = await _ownersRepository.Add(newOwner);
            if (registeredUser == null) return null;

            try
            {
                await _emailService.SendWelcomeEmail(request.Email, request.FullName);
            }
            catch (Exception ex)
            {
                // כשל בשליחת מייל ברוכים הבאים לא אמור למנוע הרשמה מוצלחת - רק מתעדים אותו.
                _logger.LogWarning(ex, "שליחת מייל ברוכים הבאים נכשלה עבור {Email}", request.Email);
            }

            var resource = _mapper.Map<OwnersResource>(registeredUser);
            resource.FullName = request.FullName;

            return resource;
        }
        // עוטפים את BCrypt.Verify כדי שסיסמה ישנה שנשמרה כטקסט גלוי (לפני המעבר להצפנה)
        // לא תפיל את הבקשה עם חריגה, אלא פשוט תיכשל כניסה כרגילה.
        private static bool IsPasswordValid(string plainPassword, string storedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, storedPassword);
            }
            catch
            {
                return false;
            }
        }

        // מאמתים את ה-ID Token מול גוגל (חתימה, תוקף, ושה-audience תואם ל-Client ID שלנו).
        // אם המשתמש כבר קיים לפי אימייל - מתחברים אליו. אם לא - נרשמים אוטומטית (בלי סיסמה מקומית,
        // כלומר לא ניתן יהיה להתחבר לחשבון הזה עם אימייל+סיסמה, רק דרך גוגל).
        public async Task<OwnersResource?> GoogleLoginAsync(string idToken)
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
            {
                _logger.LogError("Authentication:Google:ClientId לא מוגדר - לא ניתן לאמת התחברות עם גוגל.");
                return null;
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "אימות ID Token של גוגל נכשל.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(payload.Email) || !payload.EmailVerified)
                return null;

            var existingOwner = await _ownersRepository.GetByEmail(payload.Email);
            if (existingOwner != null)
            {
                var existingResource = _mapper.Map<OwnersResource>(existingOwner);
                existingResource.FullName = $"{existingOwner.FirstName} {existingOwner.LastName}".Trim();
                return existingResource;
            }

            var firstName = !string.IsNullOrWhiteSpace(payload.GivenName) ? payload.GivenName : payload.Email;
            var lastName = payload.FamilyName ?? string.Empty;

            var newOwner = new Owners
            {
                FirstName = firstName,
                LastName = lastName,
                Email = payload.Email,
                PhoneNumber = string.Empty,
                // אין סיסמה - חשבון שנוצר דרך גוגל לא ניתן לכניסה עם אימייל+סיסמה (ר' LoginAsync שדוחה סיסמה ריקה).
                Password = string.Empty,
            };

            var registeredUser = await _ownersRepository.Add(newOwner);
            if (registeredUser == null) return null;

            try
            {
                await _emailService.SendWelcomeEmail(payload.Email, $"{firstName} {lastName}".Trim());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "שליחת מייל ברוכים הבאים נכשלה עבור {Email}", payload.Email);
            }

            var resource = _mapper.Map<OwnersResource>(registeredUser);
            resource.FullName = $"{firstName} {lastName}".Trim();
            return resource;
        }
    }
}
