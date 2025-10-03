using KafkaDotnetSample.Domain.Publishers;
using KafkaDotnetSample.Infra.Publishers;
using Confluent.Kafka;
using KafkaDotnetSample.Domain.UseCases;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using KafkaDotnetSample.Domain.Consumers;
using KafkaDotnetSample.Infra.Consumers;
using KafkaDotnetSample.Domain.Repositories;
using KafkaDotnetSample.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Kafka DI setup
var kafkaBootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
if (string.IsNullOrWhiteSpace(kafkaBootstrapServers))
    throw new InvalidOperationException("KAFKA_BOOTSTRAP_SERVERS environment variable is not set.");
builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var config = new ProducerConfig { BootstrapServers = kafkaBootstrapServers };
    return new ProducerBuilder<Null, string>(config).Build();
});
builder.Services.AddScoped<IKafkaPublisher>(sp =>
{
    var producer = sp.GetRequiredService<IProducer<Null, string>>();
    var logger = sp.GetRequiredService<ILogger<KafkaPublisher>>();
    return new KafkaPublisher(producer, logger);
});

// Kafka Consumer DI setup
builder.Services.AddSingleton<IProcessedEventsRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ProcessedEventsRepository>>();
    return new ProcessedEventsRepository(logger);
});
builder.Services.AddScoped<GetProcessedEventsUseCase>();
builder.Services.AddSingleton<IKafkaConsumer>(sp =>
{
    var bootstrapServers = kafkaBootstrapServers;
    var groupId = "webapp-group";
    var topics = new[] { "login", "logout", "purchase" }; // Add more topics as needed
    var eventsRepo = sp.GetRequiredService<IProcessedEventsRepository>();
    var logger = sp.GetRequiredService<ILogger<KafkaConsumer>>();
    return new KafkaConsumer(bootstrapServers, groupId, eventsRepo, logger, topics);
});
builder.Services.AddHostedService<KafkaConsumerWorker>();

builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<LogoutUseCase>();
builder.Services.AddScoped<PurchaseUseCase>();
builder.Services.AddSingleton(new KafkaDotnetSample.Infra.HealthChecks.KafkaHealthCheck(kafkaBootstrapServers));
builder.Services.AddHealthChecks()
    .AddCheck("webapp_health", () =>
        HealthCheckResult.Healthy("Webapp is healthy"))
    .AddCheck<KafkaDotnetSample.Infra.HealthChecks.KafkaHealthCheck>("kafka_health");

// Add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KafkaDotnetSample API",
        Version = "v1",
        Description = "Minimal API endpoints for login, logout, purchase, and health."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/login", async (LoginUseCase useCase, LoginRequest req) =>
{
    var result = await useCase.ExecuteAsync(req);
    return Results.Ok(new { success = result });
})
.WithName("Login")
.WithOpenApi(op =>
{
    op.Description = "User login endpoint.";
    op.Summary = "Login";
    return op;
});

app.MapPost("/logout", async (LogoutUseCase useCase, LogoutRequest req) =>
{
    var result = await useCase.ExecuteAsync(req);
    return Results.Ok(new { success = result });
})
.WithName("Logout")
.WithOpenApi(op =>
{
    op.Description = "User logout endpoint.";
    op.Summary = "Logout";
    return op;
});

app.MapPost("/purchase", async (PurchaseUseCase useCase, PurchaseRequest req) =>
{
    var result = await useCase.ExecuteAsync(req);
    return Results.Ok(new { success = result });
})
.WithName("Purchase")
.WithOpenApi(op =>
{
    op.Description = "User purchase endpoint.";
    op.Summary = "Purchase";
    return op;
});

app.MapGet("/events", async (GetProcessedEventsUseCase useCase) =>
{
    var events = await useCase.ExecuteAsync();
    return Results.Ok(events);
})
.WithName("GetProcessedEvents")
.WithOpenApi(op =>
{
    op.Description = "Returns all processed Kafka events so far.";
    op.Summary = "Get Processed Events";
    return op;
});

app.MapHealthChecks("/health");

app.Run();
