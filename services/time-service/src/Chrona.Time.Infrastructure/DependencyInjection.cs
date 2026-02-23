using Chrona.Time.Application.Interfaces;
using Chrona.Time.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>((sp, options) =>
		{
			options
				.UseSqlServer(configuration.GetConnectionString("ExampleConnectionString"))
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
				.EnableDetailedErrors();
		});

		services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

		return services;
	}
}
