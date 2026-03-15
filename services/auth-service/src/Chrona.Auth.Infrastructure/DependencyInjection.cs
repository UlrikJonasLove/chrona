using Chrona.Auth.Application.Interfaces;
using Chrona.Auth.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Chrona.Auth.Infrastructure.Services;

namespace Chrona.Auth.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>((sp, options) =>
		{
			options
				.UseSqlServer(configuration.GetConnectionString("ChronaDb"))
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
				.EnableDetailedErrors();
		});

		services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
		services.AddScoped<ITokenService, TokenService>();

		return services;
	}
}
