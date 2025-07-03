using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.DependenceInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration configuration, string fileName) where TContext : DbContext
        {
            // Add Generic Database Context
            services.AddDbContext<TContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("eCommerceConnection"), sqlserverOption =>
                {
                    sqlserverOption.EnableRetryOnFailure();
                });
            });

            // Configure Serilog for Logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add JWT Authentication - using Dependency Injection
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UserSharedPolicies(this IApplicationBuilder app)
        {
            // Use global exception handler
            app.UseMiddleware<GlobalException>();

            // Use Middleware to listen only to API Gateway
            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
