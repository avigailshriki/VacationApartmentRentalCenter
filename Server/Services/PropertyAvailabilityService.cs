using AutoMapper;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class PropertyAvailabilityService : IPropertyAvailabilityService
    {
        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IPropertiesReposiory _propertiesReposiory;
        private readonly IMapper _mapper;

        public PropertyAvailabilityService(
            IPropertyAvailabilityRepository availabilityRepository,
            IPropertiesReposiory propertiesReposiory,
            IMapper mapper)
        {
            _availabilityRepository = availabilityRepository;
            _propertiesReposiory = propertiesReposiory;
            _mapper = mapper;
        }

        public async Task<List<PropertyAvailabilityResource?>> GetByPropertyId(int propertyId)
        {
            var list = await _availabilityRepository.GetByPropertyId(propertyId);
            return _mapper.Map<List<PropertyAvailabilityResource?>>(list);
        }

        public async Task<PropertyAvailabilityResource?> GetById(int id)
        {
            var entity = await _availabilityRepository.GetById(id);
            return _mapper.Map<PropertyAvailabilityResource?>(entity);
        }

        public async Task<PropertyAvailabilityResource?> Add(PropertyAvailabilityResource obj)
        {
            var property = await _propertiesReposiory.GetById(obj.PropertyId);
            if (property == null)
                throw new ArgumentException("הנכס לא נמצא.");

            if (obj.EndDate.Date < obj.StartDate.Date)
                throw new ArgumentException("תאריך הסיום חייב להיות אחרי תאריך ההתחלה.");

            var existingRanges = await _availabilityRepository.GetByPropertyId(obj.PropertyId);
            var hasOverlap = existingRanges.Any(r =>
                r != null &&
                r.StartDate.Date <= obj.EndDate.Date &&
                r.EndDate.Date >= obj.StartDate.Date);

            if (hasOverlap)
                throw new ArgumentException("קיימת כבר חסימה שחופפת לטווח התאריכים הזה.");

            var entity = new PropertyAvailability
            {
                PropertyId = obj.PropertyId,
                StartDate = obj.StartDate.Date,
                EndDate = obj.EndDate.Date
            };

            var added = await _availabilityRepository.Add(entity);
            return _mapper.Map<PropertyAvailabilityResource?>(added);
        }

        public async Task<bool> Delete(int id)
        {
            return await _availabilityRepository.Delete(id);
        }
    }
}
