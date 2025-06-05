using FluentValidation.AspNetCore;
using FluentValidation;
using Mapster;
using MovieStoreB.BL;
using MovieStoreB.DL;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using MovieStoreB.Controllers;
using MovieStoreB.HealthChecks;
using MovieStoreB.ServiceExtensions;
using MessagePack;
using MessagePack.Resolvers;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();

builder.Logging.AddSerilog(logger);

try
{
    logger.Information("Starting MovieStoreB application configuration...");

    // StandardResolver which handles [MessagePackObject] attributes automatically
    var options = MessagePackSerializerOptions.Standard
        .WithResolver(StandardResolver.Instance)
        .WithSecurity(MessagePackSecurity.UntrustedData);

    MessagePackSerializer.DefaultOptions = options;

    // Test serialization to make sure it works
    var testMovie = new MovieStoreB.Models.DTO.Movie
    {
        Id = "test-123",
        Title = "Test Movie",
        Year = 2024,
        ActorIds = new List<string> { "actor1", "actor2" },
        DateInserted = DateTime.UtcNow
    };

    var serialized = MessagePackSerializer.Serialize(testMovie);
    var deserialized = MessagePackSerializer.Deserialize<MovieStoreB.Models.DTO.Movie>(serialized);

    logger.Information("MessagePack configuration successful! Test movie: {Title}", deserialized.Title);

    // Add services to the container
    logger.Information("Configuring services...");

    builder.Services
        .AddConfigurations(builder.Configuration)
        .AddDataDependencies(builder.Configuration)
        .AddBusinessDependencies();

    builder.Services.AddMapster();
    builder.Services.AddValidatorsFromAssemblyContaining<TestRequest>();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks().AddCheck<SampleHealthCheck>("Sample");

    logger.Information("Services configured successfully");

    var app = builder.Build();

    logger.Information("Application built successfully, configuring pipeline...");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        logger.Information("Swagger configured for development environment");
    }

    app.MapHealthChecks("/healthz");
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    logger.Information("Application pipeline configured successfully");
    logger.Information("Starting application on URLs: {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls")?.Split(';') ?? new[] { "Not specified" }));

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Application failed to start: {ErrorMessage}", ex.Message);
    throw;
}
finally
{
    logger.Information("Application stopped");
}