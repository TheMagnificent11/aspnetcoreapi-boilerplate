using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AspNetCoreApi.Boilerplate.Infrastructure;
using Autofac;
using Autofac.Features.Variance;
using AutofacSerilogIntegration;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestManagement;
using RequestManagement.Logging;
using Serilog;
using Serilog.Events;

namespace AspNetCoreApi.Boilerplate
{
    /// <summary>
    /// Startup Base
    /// </summary>
    public abstract class AppStartupBase
    {
        private const string CorsPolicy = "CorsPolicy";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppStartupBase"/> class
        /// </summary>
        /// <param name="configuration">Conifugration settings</param>
        protected AppStartupBase(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration settings
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the API name
        /// </summary>
        protected abstract string ApiName { get; }

        /// <summary>
        /// Gets a list of the API versions
        /// </summary>
        protected abstract IList<string> ApiVersions { get; }

        /// <summary>
        /// Gets am enumerable collection of assemblies containing implmentations of
        /// <see cref="MediatR.IRequest{TResponse}"/>, <see cref="MediatR.RequestHandler{TRequest, TResponse}"/>,
        /// <see cref="MediatR.INotification"/> and <see cref="MediatR.INotificationHandler{TNotification}"/>
        /// </summary>
        protected abstract IEnumerable<Assembly> MediatrAssemblies { get; }

        /// <summary>
        /// Gets an enumerable collection of assemblies containing <see cref="AutoMapper"/> types
        /// </summary>
        protected abstract IList<Assembly> AutoMapperAssemblies { get; }

        /// <summary>
        /// Configure application
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Host environment</param>
        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(CorsPolicy);

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            this.ConfigureAuthenticationAndAuthorization(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", this.GetHealthCheckOptions());
                endpoints.MapControllers();
            });

            app.ConfigureSwagger(this.ApiName, this.ApiVersions);

            this.MigrationDatabases(app);
        }

        /// <summary>
        /// Configure dependency injection container
        /// </summary>
        /// <param name="builder">Container builder</param>
        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterLogger();

            builder.RegisterSource(new ContravariantRegistrationSource());

            this.ConfigureEntityManagement(builder);

            builder.RegisterModule(new RequestManagementModule(this.MediatrAssemblies));
        }

        /// <summary>
        /// Configures services
        /// </summary>
        /// <param name="services">Services collection</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var appSettings = this.GetSettings<ApplicationSettings>("ApplicationSettings");
            var seqSettings = this.GetSettings<SeqSettings>("SeqSettings");

            services.AddCors(o => o.AddPolicy(CorsPolicy, this.ConfigureCorsPolicy));

            services.ConfigureLogging(this.Configuration, LogEventLevel.Debug, appSettings, seqSettings);

            this.ConfigureDatabaseContexts(services);

            if (this.AutoMapperAssemblies != null && this.AutoMapperAssemblies.Any())
            {
                services.AddAutoMapper(this.AutoMapperAssemblies);
            }

            services.AddControllers(options => options.Filters.Add(new ExceptionFilter()));

            services.ConfigureProblemDetails();

            this.ConfigureHealthChecks(services.AddHealthChecks());

            services.ConfigureSwagger(this.ApiName, this.ApiVersions);
        }

        /// <summary>
        /// Migrates database of the specified <see cref="DbContext"/> type to the latest version
        /// </summary>
        /// <typeparam name="T">Type of <see cref="DbContext"/> to use to migrate database</typeparam>
        /// <param name="app">Application builder</param>
        protected static void MigrationDatabase<T>(IApplicationBuilder app)
            where T : DbContext
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
                dbContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Get strongly-typed settings from appsettings.json
        /// </summary>
        /// <typeparam name="T">Type to which to bind the settings</typeparam>
        /// <param name="configurationSection">Name of the configuration section containing the settings</param>
        /// <returns>Strongly-typed settings</returns>
        protected T GetSettings<T>(string configurationSection)
            where T : class, new()
        {
            var settings = new T();

            this.Configuration.Bind(configurationSection, settings);

            return settings;
        }

        /// <summary>
        /// Configures the CORS policy
        /// </summary>
        /// <param name="policyBuilder">CORS policy builder</param>
        protected virtual void ConfigureCorsPolicy(CorsPolicyBuilder policyBuilder)
        {
            if (policyBuilder is null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            var origins = (this.Configuration.GetValue<string>("AllowedOrigins") ?? string.Empty)
                .Split(';')
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            policyBuilder = origins.Any() ? policyBuilder.WithOrigins(origins) : policyBuilder.AllowAnyOrigin();

            policyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }

        /// <summary>
        /// Configures authentication
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <example>
        /// app.UseAuthentication();
        /// app.UseAuthorization();
        /// </example>
        protected virtual void ConfigureAuthenticationAndAuthorization(IApplicationBuilder app)
        {
        }

        /// <summary>
        /// Migrates databases
        /// </summary>
        /// <param name="app">Application builder</param>
        protected virtual void MigrationDatabases(IApplicationBuilder app)
        {
        }

        /// <summary>
        /// Configures <see cref="EntityManagement"/>
        /// </summary>
        /// <param name="builder">Container builder</param>
        protected virtual void ConfigureEntityManagement(ContainerBuilder builder)
        {
        }

        /// <summary>
        /// Configures the database contexts used by this application
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <example>
        /// services.AddDbContextPool&lt;ApplicationDbContext&gt;(options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
        /// </example>
        protected virtual void ConfigureDatabaseContexts(IServiceCollection services)
        {
        }

        /// <summary>
        /// Configures health checks
        /// </summary>
        /// <param name="builder">Health check builder</param>
        protected virtual void ConfigureHealthChecks(IHealthChecksBuilder builder)
        {
        }

        /// <summary>
        /// Gets the health check otions
        /// </summary>
        /// <returns>Health check options</returns>
        protected virtual HealthCheckOptions GetHealthCheckOptions()
        {
            var appSettings = this.GetSettings<ApplicationSettings>("ApplicationSettings");
            var responseWriter = new HealthCheckResponseWriter(appSettings);

            return new HealthCheckOptions
            {
                ResponseWriter = responseWriter.WriteToResponse
            };
        }
    }
}
