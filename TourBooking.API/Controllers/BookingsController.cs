using Microsoft.AspNetCore.Mvc;
using TourBooking.Application.DTOs;
using TourBooking.Application.Interfaces;
using TourBooking.Domain.Entities;

namespace TourBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var dto = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                BookingDate = b.BookingDate,
                NumberOfPeople = b.NumberOfPeople,
                TourId = b.TourId,
                TourTitle = b.Tour.Title
            }).ToList();
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                if (booking == null) return NotFound();

                var dto = new BookingDto
                {
                    Id = booking.Id,
                    CustomerName = booking.CustomerName,
                    CustomerEmail = booking.CustomerEmail,
                    BookingDate = booking.BookingDate,
                    NumberOfPeople = booking.NumberOfPeople,
                    TourId = booking.TourId,
                    TourTitle = booking.Tour.Title
                };
                return Ok(dto);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingDto dto)
        {
            var booking = new Booking
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                BookingDate = dto.BookingDate,
                NumberOfPeople = dto.NumberOfPeople,
                TourId = dto.TourId
            };
            await _bookingRepository.AddAsync(booking);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            await _bookingRepository.DeleteAsync(booking);
            return NoContent();
        }
    }
}
