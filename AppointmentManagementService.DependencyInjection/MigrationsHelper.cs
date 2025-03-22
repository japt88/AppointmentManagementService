using AppointmentManagementService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentManagementService.DependencyInjection
{
    public static class MigrationsHelper
    {
        public static void ApplyMigrations(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<AppointmentDbContext>();
                dbContext.Database.Migrate(); // Apply pending migrations
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
