﻿using System.Text.Json;
using Ufynd.Core.Configurations;
using Ufynd.Reporting.Api.Middlewares;

namespace Ufynd.Reporting.Api.Extensions;

public static class BuilderExtension
{
    public static WebApplication BuildApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors();
        builder.Services.AddControllers().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);
        builder.Services.AddCustomServicesAndConfigurations(builder.Configuration);
        builder.Services.AddHealthChecks();
        builder.Services.AddActorSystem(builder.Configuration);
        
        return builder.Build();
    }

    public static void RunApplication(this WebApplication application)
    {
        
        // Configure the HTTP request pipeline.
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI(s => 
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Ufynd Reporting Api");
            });
        }

        application.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

        application.UseRouting();
        application.ConfigureGlobalHandler(application.Logger);
        application.UseHttpsRedirection();
        application.UseAuthorization();
        application.MapControllers();

        application.Run();
    }
}