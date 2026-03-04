using Microsoft.AspNetCore.Mvc;
using TourBooking.API.Common;
using TourBooking.Application.DTOs;
using TourBooking.Application.Features.Tours;
using TourBooking.Domain.Entities;

namespace TourBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly ITourService _tourService;

        public ToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tours = await _tourService.GetTourListAsync();
            return Ok(tours);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tour = await _tourService.GetByIdAsync(id);
            if (tour == null) return NotFound();
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
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TourDto dto)
        {
            var id = await _tourService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TourDto tour)
        {
            
            var result = await _tourService.UpdateAsync(tour);
            if(result > 0)
            {
                return Ok("Success");
            }
            else
            {
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int result = await _tourService.DeleteAsync(id);
            return (result > 0 ? Ok() : NoContent());
        }
    }
}
