using AutoMapper;
using Core.Models;
using Core.Repository;
using Core.Resources;
using Core.Services;
using Data.DataRepository;

namespace Services
{
    public class PropertiesService : IPropertiesService
    {
        private readonly IPropertiesReposiory _propertiesReposiory;
        private readonly IImagesRepository _imageRepository;
        private readonly IMapper _mapper;

        public PropertiesService(IPropertiesReposiory propertiesReposiory, IImagesRepository imageRepository, IMapper mapper)
        {
            _propertiesReposiory = propertiesReposiory;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }
        public async Task<List<PropertiesResource?>> GetAll()
        {
            var properties = await _propertiesReposiory.GetAll();
            return _mapper.Map<List<PropertiesResource?>>(properties);
        }
        public async Task<PropertiesResource?> GetById(int id)
        {
            var property = await _propertiesReposiory.GetById(id);
            return _mapper.Map<PropertiesResource?>(property);
        }
        public async Task<PropertiesResource?> Add(Properties resource)
        {
            var addedProperty = await _propertiesReposiory.Add(resource);
            return _mapper.Map<PropertiesResource>(addedProperty);
        }
        public async Task<PropertiesResource> AddPropertyByOwnerID(int ownerId, Properties property)
        {
            var addedProperty = await _propertiesReposiory.AddPropertyByOwnerID(ownerId, property);
            return _mapper.Map<PropertiesResource>(addedProperty);
        }
        public async Task<bool> Delete(int id)
        {
            return await _propertiesReposiory.Delete(id);
        }
        public async Task<PropertiesResource?> Update(int id, Properties properties)
        {
            var updated = await _propertiesReposiory.Update(id, properties);
            return _mapper.Map<PropertiesResource?>(updated);
        }
        public async Task<List<PropertiesResource>> GetFiltered(string? title, string? city, double? maxPrice, int? capacity)
        {
            var list = await _propertiesReposiory.GetFiltered(title, city, maxPrice, capacity);
            return _mapper.Map<List<PropertiesResource>>(list);
        }
        public async Task<List<PropertiesResource>> GetOwnerProperties(int ownerId)
        {
            var properties = await _propertiesReposiory.GetPropertiesByOwnerId(ownerId);
            return _mapper.Map<List<PropertiesResource>>(properties);
        }
        public async Task<List<Review?>> GetPropertyReviews(int propertyId)
        {
            var reviews = await _propertiesReposiory.GetPropertyReviews(propertyId);
            return reviews;
        }
        public async Task<PropertiesResource?> ChangeStatus(int propertyID)
        {
            PropertiesResource? propertiesResource = await this.GetById(propertyID);

            if (propertiesResource == null)
            {
                return null;
            }
            propertiesResource.IsAvailable = !propertiesResource.IsAvailable;

            var propertyModel = _mapper.Map<Properties>(propertiesResource);

            var updatedResource = await this.Update(propertyID, propertyModel);

            return updatedResource;
        }
        public async Task<Properties?> CreatePropertyAsync(PropertyAddDto propertyDto)
        {
            var property = _mapper.Map<Properties>(propertyDto);
            return await _propertiesReposiory.Add(property);
        }
        public async Task SaveImageUrlAsync(int propertyId, string imageUrl)
        {
            var imageEntity = new Images { PropertyId = propertyId, ImageUrl = imageUrl };
            await _imageRepository.Add(imageEntity);
        }
    }
}