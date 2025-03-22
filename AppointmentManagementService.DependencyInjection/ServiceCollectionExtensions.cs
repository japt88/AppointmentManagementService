using AppointmentManagementService.Data;
using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services;
using AppointmentManagementService.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentManagementService.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<AppointmentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));                      

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Unit of Work
            services.AddScoped<IAppointmentUnitOfWork, AppointmentUnitOfWork>();

            // Register Repositories
            services.AddScoped<IGenericRepository<Patient>, GenericRepository<Patient>>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            // Register Services
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();

            // Register AutoMapper Profiles
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }

        public static void InitializeDatabase(this IServiceProvider serviceProvider)
        {
            MigrationsHelper.ApplyMigrations(serviceProvider);
        }
    }
}
