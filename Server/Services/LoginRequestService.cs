using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;

namespace Services
{
    public class LoginRequestService : ILoginRequestService
    {
        private readonly IOwnersRepository _ownersRepository;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;

        public LoginRequestService(IOwnersRepository ownersRepository, EmailService emailService, IMapper mapper)
        {
            _ownersRepository = ownersRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<OwnersResource?> LoginAsync(LoginRequest request)
        {
            var owner = await _ownersRepository.GetByEmailAndPassword(request.Email, request.Password);
            if (owner == null) return null;

            var resource = _mapper.Map<OwnersResource>(owner);
            return resource;
        }
        public async Task<OwnersResource?> RegisterAsync(RegisterRequest request)
        {
            var existingOwner = await _ownersRepository.GetByEmail(request.Email);
            if (existingOwner != null)
            {
                throw new Exception("משתמש עם אימייל זה כבר קיים במערכת.");
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
                Password = request.Password,
            };

            var registeredUser = await _ownersRepository.Add(newOwner);
            if (registeredUser == null) return null;

            await _emailService.SendWelcomeEmail(request.Email, request.FullName);

            var resource = _mapper.Map<OwnersResource>(registeredUser);
            resource.FullName = request.FullName;

            return resource;
        }
    }
}