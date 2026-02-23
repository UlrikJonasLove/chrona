using System.Diagnostics.CodeAnalysis;

namespace Chrona.Time.Presentation.Infrastructure;

public static class IEndpointRouteBuilderExtensions
{
	public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
	{
		builder.MapGet(pattern, handler)
			.WithName(CreateEndpointName(handler));

		return builder;
	}

	public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
	{
		builder.MapPost(pattern, handler)
			.WithName(CreateEndpointName(handler));

		return builder;
	}

	public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
	{
		builder.MapPut(pattern, handler)
			.WithName(CreateEndpointName(handler));

		return builder;
	}

	public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern)
	{
		builder.MapDelete(pattern, handler)
			.WithName(CreateEndpointName(handler));

		return builder;
	}

	private static string CreateEndpointName(Delegate handler)
	{
		var declaringTypeName = handler.Method.DeclaringType?.Name ?? "Unknown";
		return $"{declaringTypeName}_{handler.Method.Name}";
	}
}
