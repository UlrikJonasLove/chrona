using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Chrona.Time.Application.Behaviors;

namespace Chrona.Time.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
		});

		return services;
	}
}
