using Chrona.Auth.Presentation;
using Chrona.Auth.Application;
using Chrona.Auth.Infrastructure;
using Chrona.Auth.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddApplicationServices(builder.Configuration["RefreshTokenPepper"]!)
	.AddInfrastructureServices(builder.Configuration)
	.AddPresentationServices();

var application = builder.Build();

if (application.Environment.IsDevelopment())
	application
		.UseSwagger()
		.UseSwaggerUI();

application
	.UseHttpsRedirection()
	.UseAuthorization();

application
	.MapEndpoints()
	.Run();
