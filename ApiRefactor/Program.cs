using ApiRefactor.Data;
using ApiRefactor.Messaging;
using ApiRefactor.Messaging.Consumers;
using ApiRefactor.Options;
using ApiRefactor.Services;
using ApiRefactor.Swagger;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<WaveDatabaseOptions>(builder.Configuration.GetSection(WaveDatabaseOptions.SectionName));
builder.Services.Configure<MassTransitOptions>(builder.Configuration.GetSection(MassTransitOptions.SectionName));

builder.Services.AddDbContext<WavesDbContext>((sp, options) =>
{
    var waveDb = sp.GetRequiredService<IOptions<WaveDatabaseOptions>>().Value;
    options.UseSqlite(waveDb.ConnectionString);
});

builder.Services.AddSingleton<WaveDbInitializer>();
builder.Services.AddScoped<IWaveRepository, EfWaveRepository>();
builder.Services.AddScoped<IWaveService, WaveService>();
builder.Services.AddScoped<IWaveEventPublisher, MassTransitWaveEventPublisher>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

var massTransitSection = builder.Configuration.GetSection(MassTransitOptions.SectionName);
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<WaveUpsertedConsumer>();

    var mt = massTransitSection.Get<MassTransitOptions>() ?? new MassTransitOptions();
    if (string.Equals(mt.Transport, "AzureServiceBus", StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrWhiteSpace(mt.AzureServiceBusConnectionString))
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(mt.AzureServiceBusConnectionString);
            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
    }
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<WaveDbInitializer>();
    var db = scope.ServiceProvider.GetRequiredService<WavesDbContext>();
    await initializer.EnsureCreatedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Waves API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program;
