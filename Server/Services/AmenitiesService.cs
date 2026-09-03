using AutoMapper;
using Core.Mapping;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;
using Data.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AmenitiesService : IAmenitiesService
    {
        private readonly IAmenitiesRepository _amenitiesRepository;
        private readonly IMapper _mapper;

        public AmenitiesService(IAmenitiesRepository amenitiesRepository, IMapper mapper)
        {
            _amenitiesRepository = amenitiesRepository;
            _mapper = mapper;
        }
        public async Task<AmenitiesResource?> Add(Amenities obj)
        {
            var addedEntity = await _amenitiesRepository.Add(obj);

            if (addedEntity == null)
                return null;
            return _mapper.Map<AmenitiesResource>(addedEntity);
        }
        public async Task<bool> Delete(int id)
        {
            return await _amenitiesRepository.Delete(id);
        }
        public async Task<List<AmenitiesResource?>> GetAll()
        {
            var entities = await _amenitiesRepository.GetAll();
            return entities.Select(e => _mapper.Map<AmenitiesResource?>(e)).ToList();
        }
        public async Task<AmenitiesResource?> GetById(int id)
        {
            var entity = await _amenitiesRepository.GetById(id);
            return _mapper.Map<AmenitiesResource>(entity);
        }
        public async Task<AmenitiesResource?> Update(int id, Amenities obj)
        {
            var updatedEntity = await _amenitiesRepository.Update(id, obj);
            return _mapper.Map<AmenitiesResource>(updatedEntity);
        }
    }
}
