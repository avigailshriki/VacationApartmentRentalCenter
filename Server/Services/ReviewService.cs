using AutoMapper;
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
    public class ReviewService : IReviewService
    {
        private readonly IPropertiesReposiory _propertiesReposiory;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        public ReviewService(IPropertiesReposiory propertiesReposiory, IReviewRepository reviewRepository, IMapper mapper)
        {
            _propertiesReposiory = propertiesReposiory;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }
        public async Task<ReviewResource?> Add(ReviewResource obj)
        {
            var property = await _propertiesReposiory.GetById(obj.PropertyId);
            if (property == null)
            {
                return null;
            }
            var review = _mapper.Map<Review>(obj);

            var newReview = await _reviewRepository.Add(review);

            return _mapper.Map<ReviewResource>(newReview);
        }
        public async Task<bool> Delete(int id)
        {
            return await _reviewRepository.Delete(id);
        }
        public async Task<List<ReviewResource?>> GetAll()
        {
            var reviews = await _reviewRepository.GetAll();
            return _mapper.Map<List<ReviewResource?>>(reviews);
        }
        public async Task<ReviewResource?> GetById(int id)
        {
            var review = await _reviewRepository.GetById(id);
            return _mapper.Map<ReviewResource?>(review);
        }
        public async Task<ReviewResource?> Update(int id, ReviewResource obj)
        {
            var review = await _reviewRepository.GetById(id);
            if (review == null)
            {
                return null;
            }
            _mapper.Map(obj, review);
            var updatedReview = await _reviewRepository.Update(id, review);
            return _mapper.Map<ReviewResource?>(updatedReview);
        }
    }
}
