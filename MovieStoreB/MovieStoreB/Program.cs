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
using MovieStoreB.Models.DTO;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme:
        AnsiConsoleTheme.Code)
    .CreateLogger();

builder.Logging.AddSerilog(logger);

// 🔥 FIXED MESSAGEPACK CONFIGURATION (removed GeneratedResolver):
StaticCompositeResolver.Instance.Register(
    StandardResolver.Instance
);

var options = MessagePackSerializerOptions.Standard
    .WithResolver(StaticCompositeResolver.Instance);

MessagePackSerializer.DefaultOptions = options;

// Test serialization
try
{
    var testMovie = new Movie { Id = "test", Title = "Test Movie", Year = 2024 };
    var serialized = MessagePackSerializer.Serialize(testMovie);
    var deserialized = MessagePackSerializer.Deserialize<Movie>(serialized);
    logger.Information("MessagePack serialization test successful!");
}
catch (Exception ex)
{
    logger.Error(ex, "MessagePack serialization test failed!");
}

// Add services to the container.
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();