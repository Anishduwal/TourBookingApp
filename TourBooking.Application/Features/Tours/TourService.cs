using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourBooking.Application.DTOs;
using TourBooking.Application.Interfaces;
using TourBooking.Domain.Entities;

namespace TourBooking.Application.Features.Tours
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _repository;
        private readonly ICacheService _cacheService;

        public TourService(
            ITourRepository repository,
            ICacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<List<TourDto>> GetTourListAsync()
        {
            string cacheKey = "tour:list";

            // 1️⃣ Check Redis
            var cached = await _cacheService.GetAsync<List<TourDto>>(cacheKey);
            if (cached != null)
                return cached;

            // 2️⃣ Fetch from DB
            var tours = await _repository.GetAllAsync();

            var tourDto = tours.Select(t => new TourDto
            {
                Id = t.Id,
                Title = t.Title,
                Location = t.Location,
                Description = t.Description,
                Price = t.Price,
                DurationInHours = t.DurationInHours,
                IsActive = t.IsActive
            }).ToList();

            // 3️⃣ Store in Redis
            await _cacheService.SetAsync(cacheKey, tourDto, TimeSpan.FromMinutes(10));

            return tourDto;
        }
        public async Task<TourDto?> GetByIdAsync(int id)
        {
            string cacheKey = $"tour:{id}";

            var cached = await _cacheService.GetAsync<TourDto>(cacheKey);
            if (cached != null)
                return cached;

            var tour = await _repository.GetByIdAsync(id);
            if (tour == null)
                return null;

            var dto = new TourDto
            {
                Id = tour.Id,
                Title = tour.Title,
                Location = tour.Location,
                Description = tour.Description,
                Price = tour.Price,
                DurationInHours = tour.DurationInHours,
                IsActive = tour.IsActive
            };

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

            return dto;
        }
        public async Task<int> CreateAsync(TourDto dto)
        {
            // 1️⃣ Validation (business rule example)
            if (dto.Price <= 0)
                throw new Exception("Price must be greater than zero");

            // 2️⃣ Mapping
            var tour = new Tour
            {
                Title = dto.Title,
                Location = dto.Location,
                Description = dto.Description,
                Price = dto.Price,
                DurationInHours = dto.DurationInHours,
                IsActive = dto.IsActive
            };

            // 3️⃣ Save
            await _repository.AddAsync(tour);

            // 4️⃣ Optional: Cache invalidation
            await _cacheService.RemoveAsync("tour:list");

            return tour.Id;
        }
        public async Task<int> UpdateAsync(TourDto dto)
        {
            var existingTour = await _repository.GetByIdAsync(dto.Id);
            if (existingTour == null) return 0;

            existingTour.Title = dto.Title;
            existingTour.Location = dto.Location;
            existingTour.Description = dto.Description;
            existingTour.Price = dto.Price;
            existingTour.DurationInHours = dto.DurationInHours;
            existingTour.IsActive = dto.IsActive;
            await _repository.UpdateAsync(existingTour);
            // 🔥 Invalidate caches
            await _cacheService.RemoveAsync($"tour:{existingTour.Id}");
            await _cacheService.RemoveAsync("tour:list");
            return 1;
        }
        public async Task<int> DeleteAsync(int id)
        {
            var existingTour = await _repository.GetByIdAsync(id);
            if (existingTour == null) return 0;
            await _repository.DeleteAsync(existingTour);
            await _cacheService.RemoveAsync("tour:list");
            return 1;
        }

    }
}
