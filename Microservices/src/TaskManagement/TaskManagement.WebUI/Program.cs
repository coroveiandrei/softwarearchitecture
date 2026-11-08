// create web-app
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TaskManagement.Application.Commands.ToDoCommands;
using TaskManagement.WebUI;
using TaskManagement.Infrastructure.Bootstrap;
using TaskManagement.Application.Bootstrap;
using TaskManagement.Persistance.Bootstrap;
using Google.Api;
using System.Configuration;
using TaskManagement.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManagement.Domain.Entities.Authentication;
using TaskManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.WebUI.FailureSimulation;

var builder = WebApplication.CreateBuilder(args);


var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3700";
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
builder.Services.AddDaprClient(builder => builder
    .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

builder.Services.AddControllers();
builder.Services.AddSingleton(_ => PlacementEventFailureSimulator.FromEnvironment());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<AddToDoCommandValidator>());
builder.Services.AddValidatorsFromAssembly(typeof(AddToDoCommandValidator).Assembly);
builder.Services.RegisterInfrastructureComponents();
builder.Services.RegisterApplicationServices();
builder.Services.RegisterRepositories();
builder.Services.RegisterWebAPIServices();
builder.Services.AddIdentity<User, ApplicationRole>()
                  .AddEntityFrameworkStores<ToDoDbContext>()
                  .AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("ToDoDatabase");
builder.Services.AddDbContext<ToDoDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddLocalization();
builder.Logging.AddApplicationInsights();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    dbContext.Database.Migrate();
}

// configure web-app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCloudEvents();

// configure routing
app.MapControllers();
app.MapSubscribeHandler();

// let's go!
app.Run("http://localhost:7000");
