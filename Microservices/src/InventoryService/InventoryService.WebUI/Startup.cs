using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InventoryService.Infrastructure.Bootstrap;
using InventoryService.Persistance.Bootstrap;
using Joonasw.AspNetCore.SecurityHeaders;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using InventoryService.Persistance;
using InventoryService.Application.Bootstrap;
using Microsoft.Extensions.Options;
using FluentValidation;
using Microsoft.AspNetCore.Antiforgery;
using InventoryService.Infrastructure.Services;
using InventoryService.Application.Commands.StockCommands;
using InventoryService.Application.RemoteServices;
using InventoryService.Infrastructure.RemoteServices;
using Dapr.Client;

namespace InventoryService.WebUI
{
    public class Startup
    {
        private const string ReportViolationsTo = "/csp-report";
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddDistributedMemoryCache();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.MaxAge = TimeSpan.FromDays(365);
            });

            var connectionString = Configuration.GetConnectionString("ToDoDatabase");
            services.AddDbContext<ToDoDbContext>(options => options.UseSqlServer(connectionString));
            var tableString = Configuration.GetConnectionString("TableConnectionString");
            var serviceBusString = Configuration.GetConnectionString("ServiceBusString");


            services.AddTransient<ITableStorageConnection, TableStorageConnection>(service => new TableStorageConnection(tableString));
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-GB")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en-GB", uiCulture: "en-GB");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                var defaultRequestProvider =
                    options.RequestCultureProviders.FirstOrDefault(rcp =>
                        rcp.GetType() == typeof(AcceptLanguageHeaderRequestCultureProvider));
                if (defaultRequestProvider != null)
                    options.RequestCultureProviders.Remove(defaultRequestProvider);

                options.RequestCultureProviders.Insert(0,
                    new AcceptLanguageHeaderRequestCultureProvider()
                    {
                        Options = options
                    });
            });

            var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "4000";
            var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "40000";
            services.AddDaprClient(builder => builder
                .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
                .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddValidatorsFromAssembly(typeof(StockCommandHandler).Assembly);
            services.RegisterInfrastructureComponents();
            services.RegisterApplicationServices();
            services.RegisterRepositories();
            services.RegisterWebAPIServices();

            services.AddSingleton<IPlacementService>(_ =>
                new PlacementService(DaprClient.CreateInvokeHttpClient(
                "placementservice", $"http://localhost:{daprHttpPort}")));

            services.AddCsp(nonceByteAmount: 32);
            
            services.AddControllers().AddDapr();

            services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews();
  
            services.AddSwaggerGen();

            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IAntiforgery antiforgery)//, IAsyncQueryService asyncQueryService, IQueryHelperService queryHelperService, AppConfigurations appConfigurations)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api-docs/v1/swagger.json", "My API V1");
            });

            var supportedCultures = new[] { "en-GB" };
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                SupportedCultures = supportedCultures.Select(n => CultureInfo.GetCultureInfo(n)).ToList(),
                SupportedUICultures = supportedCultures.Select(n => CultureInfo.GetCultureInfo(n)).ToList(),
            };
            app.UseRequestLocalization(requestLocalizationOptions);
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.Use(next => context =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = true, Path = "/" });
                //setting HttpOnly=true,Secure=true will make browser cookie secure 
                return next(context);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1;mode=block");
                context.Response.Headers["Cache-control"] = "private,no-cache,no-store, must-revalidate ,max-age=0";
                await next();
            });

            // Content Security Policy
            app.UseCsp(csp =>
            {
                csp.ByDefaultAllow
                    .FromSelf();

                csp.AllowFormActions
                .ToSelf();

                // Allow JavaScript from:
                csp.AllowScripts
                    .FromSelf() //This domain
                    .AddNonce()
                    .AllowUnsafeInline();

                // CSS allowed from:
                csp.AllowStyles
                    .FromSelf()
                  .AllowUnsafeInline();

                csp.AllowImages
                    .FromSelf();

                // HTML5 audio and video elemented sources can be from:
                csp.AllowAudioAndVideo
                    .FromNowhere();

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .FromNowhere(); //Nowhere, no iframes allowed

                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections
                    .ToSelf();

                // Allow fonts to be downloaded from:
                csp.AllowFonts
                   .FromSelf();

                // Allow object, embed, and applet sources from:
                csp.AllowPlugins
                    .FromNowhere();

                // Allow other sites to put this in an iframe?
                csp.AllowFraming
                    .FromNowhere(); // Block framing on other sites, equivalent to X-Frame-Options: DENY

                // Do not block violations, only report
                // This is a good idea while testing your CSP
                // Remove it when you know everything will work
                csp.SetReportOnly();
                // Where should the violation reports be sent to?
                csp.ReportViolationsTo(ReportViolationsTo);

                // Do not include the CSP header for requests to the /api endpoints
                csp.OnSendingHeader = context =>
                {
                    context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
                    return Task.CompletedTask;
                };
            });
        }
    }
}
