using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourBooking.Application.Features.Tours;
using TourBooking.Application.Interfaces;
using TourBooking.Infrastructure.Repositories;
using TourBooking.Infrastructure.Services;

namespace TourBooking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ITourService, TourService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            return services;
        }
    }
}
