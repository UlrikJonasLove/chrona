using Chrona.Auth.Presentation.Infrastructure;

namespace Chrona.Auth.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentationServices(this IServiceCollection services)
	{
		services.AddHealthChecks();

		services
			.AddHttpContextAccessor()
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddAuthorization()
			.AddExceptionHandler<GlobalExceptionHandler>()
			.AddProblemDetails();

		return services;
	}
}
