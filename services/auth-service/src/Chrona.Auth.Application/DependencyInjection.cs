using System.Reflection;
using Chrona.Auth.Application.Behaviors;
using Chrona.Auth.Application.Common.Helpers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Chrona.Auth.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, string refreshTokenPepper)
	{
		services.AddSingleton(_ => new RefreshTokenHelper(refreshTokenPepper));

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
		});

		return services;
	}
}
