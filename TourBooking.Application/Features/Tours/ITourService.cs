using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourBooking.Application.DTOs;
using TourBooking.Domain.Entities;

namespace TourBooking.Application.Features.Tours
{
    public interface ITourService
    {
        Task<List<TourDto>> GetTourListAsync();
        Task<TourDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(TourDto dto);
        Task<int> UpdateAsync(TourDto tour);
        Task<int> DeleteAsync(int id);
    }
}
