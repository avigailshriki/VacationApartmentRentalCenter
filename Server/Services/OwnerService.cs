using AutoMapper;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class OwnerService : IOwnersService
    {
        private readonly IOwnersRepository ownersRepository;
        private readonly IMapper _mapper;

        public OwnerService(IOwnersRepository ownersRepository, IMapper mapper)
        {
            this.ownersRepository = ownersRepository;
            _mapper = mapper;
        }
        public async Task<OwnersResource?> Add(Owners obj)
        {
            var addedEntity = await ownersRepository.Add(obj);
            return _mapper.Map<OwnersResource>(addedEntity);
        }
        public async Task<bool> Delete(int id)
        {
            return await ownersRepository.Delete(id);
        }
        public async Task<List<OwnersResource?>> GetAll()
        {
            var entities = await ownersRepository.GetAll();
            return _mapper.Map<List<OwnersResource?>>(entities);
        }
        public async Task<OwnersResource?> GetById(int id)
        {
            var entity = await ownersRepository.GetById(id);
            return _mapper.Map<OwnersResource?>(entity);
        }

        public async Task<OwnersResource?> Update(int id, Owners obj)
        {
            var updatedEntity = await ownersRepository.Update(id, obj);

            if (updatedEntity == null)
                return null;

            return _mapper.Map<OwnersResource>(updatedEntity);
        }
        public async Task<OwnersResource?> LoginAsync(LoginRequest request)
        {
            var owner = await ownersRepository.GetByEmailAndPassword(request.Email, request.Password);

            if (owner == null)
            {
                return null;
            }
            var resource = _mapper.Map<OwnersResource>(owner);
            resource.FullName = $"{owner.FirstName} {owner.LastName}".Trim();

            return resource;
        }
    }
}