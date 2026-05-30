using PatiantMicroService.Domain.Contracts;
using PatiantMicroService.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace PatiantMicroServiceWeb.Web.Extentions
{
    public static class WebApplicationRegisteration
    {
        public static async Task<WebApplication> MigratePatiantDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var DbContextService = scope.ServiceProvider.GetRequiredService<PatientDbContext>();
            var pendingMigrations = await DbContextService.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await DbContextService.Database.MigrateAsync();
            }
            return app;
        }
    }
}
