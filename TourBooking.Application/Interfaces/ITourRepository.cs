using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourBooking.Domain.Entities;

namespace TourBooking.Application.Interfaces
{
    public interface ITourRepository
    {
        Task<List<Tour>> GetAllAsync();
        Task<Tour?> GetByIdAsync(int id);
        Task AddAsync(Tour tour);
        Task UpdateAsync(Tour tour);
        Task DeleteAsync(Tour tour);
    }
}
