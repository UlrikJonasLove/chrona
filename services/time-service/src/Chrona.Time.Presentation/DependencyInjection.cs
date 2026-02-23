using Chrona.Time.Presentation.Infrastructure;

namespace Chrona.Time.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentationServices(this IServiceCollection services)
	{
		services.AddHealthChecks();

		services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddAuthorization()
			.AddExceptionHandler<GlobalExceptionHandler>()
			.AddProblemDetails();

		return services;
	}
}
