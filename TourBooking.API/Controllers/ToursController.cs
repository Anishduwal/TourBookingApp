using Microsoft.AspNetCore.Mvc;
using TourBooking.Application.DTOs;
using TourBooking.Application.Interfaces;
using TourBooking.Domain.Entities;

namespace TourBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly ITourRepository _tourRepository;

        public ToursController(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tours = await _tourRepository.GetAllAsync();
            var dto = tours.Select(t => new TourDto
            {
                Id = t.Id,
                Title = t.Title,
                Location = t.Location,
                Price = t.Price,
                DurationInHours = t.DurationInHours,
                IsActive = t.IsActive
            }).ToList();
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            var dto = new TourDto
            {
                Id = tour.Id,
                Title = tour.Title,
                Location = tour.Location,
                Price = tour.Price,
                DurationInHours = tour.DurationInHours,
                IsActive = tour.IsActive
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TourDto dto)
        {
            var tour = new Tour
            {
                Title = dto.Title,
                Location = dto.Location,
                Price = dto.Price,
                DurationInHours = dto.DurationInHours,
                IsActive = dto.IsActive
            };
            await _tourRepository.AddAsync(tour);
            return CreatedAtAction(nameof(GetById), new { id = tour.Id }, tour);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TourDto dto)
        {
            var existingTour = await _tourRepository.GetByIdAsync(id);
            if (existingTour == null) return NotFound();

            existingTour.Title = dto.Title;
            existingTour.Location = dto.Location;
            existingTour.Price = dto.Price;
            existingTour.DurationInHours = dto.DurationInHours;
            existingTour.IsActive = dto.IsActive;

            await _tourRepository.UpdateAsync(existingTour);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingTour = await _tourRepository.GetByIdAsync(id);
            if (existingTour == null) return NotFound();

            await _tourRepository.DeleteAsync(existingTour);
            return NoContent();
        }
    }
}
