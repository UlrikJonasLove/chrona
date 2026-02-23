using Chrona.Time.Presentation;
using Chrona.Time.Application;
using Chrona.Time.Infrastructure;
using Chrona.Time.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddApplicationServices()
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
