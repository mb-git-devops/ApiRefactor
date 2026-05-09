using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiRefactor.Swagger;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "Waves API",
                Version = description.ApiVersion.ToString(),
                Description = @"Fictional Coles-style HTTP API for waves of orders routed to stores for picking. Resources are versioned in the URL path. Successful upserts publish a WaveUpserted integration event via MassTransit (in-memory bus locally, or Azure Service Bus when configured)."
            });
        }
    }
}
