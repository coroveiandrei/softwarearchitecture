// create web-app
using PlacementService.Repositories;
using PlacementService.FailureSimulation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPlacemementRepository, PlacementRepository>();
builder.Services.AddSingleton(_ => ReservationFailureSimulator.FromEnvironment());

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
builder.Services.AddDaprClient(builder => builder
    .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

builder.Services.AddControllers().AddDapr();
builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.AddApplicationInsights();

var app = builder.Build();
app.Logger.LogInformation("Reservation failure simulator configured | PLACEMENT_FAILURE_COUNT={FailureCount} (0=disabled, -1=always fail)", Environment.GetEnvironmentVariable("PLACEMENT_FAILURE_COUNT") ?? "0");

// configure web-app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCloudEvents();

// configure routing
app.MapControllers();

// let's go!
app.Run("http://localhost:6000");
