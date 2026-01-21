using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourBooking.Application.Interfaces;
using TourBooking.Domain.Entities;

namespace TourBooking.Infrastructure.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tour>> GetAllAsync()
            => await _context.Tours.AsNoTracking().ToListAsync();

        public async Task<Tour?> GetByIdAsync(int id)
            => await _context.Tours.FindAsync(id);

        public async Task AddAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tour tour)
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tour tour)
        {
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
        }
    }
}
