using BookStore.Infrastructure.Interceptors;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure;

public static class DependencyInjection 
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options ) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

            options.AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
        });

        return services;
    }
}
