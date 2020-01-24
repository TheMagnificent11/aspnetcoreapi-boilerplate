using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using AspNetCoreApi.Boilerplate.Configuration;
using Autofac;
using Autofac.Features.Variance;
using AutofacSerilogIntegration;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestManagement;
using RequestManagement.Logging;
using Serilog.Events;

namespace AspNetCoreApi.Boilerplate
{
    /// <summary>
    /// Startup Base
    /// </summary>
    public abstract class AppStartupBase
    {
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
        public virtual void Configure([NotNull] IApplicationBuilder app, [NotNull] IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                this.ConfigureLocalDevelopmentEnvironment(app);
            }
            else
            {
                this.ConfigureDeployedEnvironment(app);
            }

            this.ConfigureHttpsRedirection(app);
            this.ConfigureRoutingAndEndpoints(app);
            this.ConfigureAuthenticationAndAuthorization(app);
            app.ConfigureSwagger(this.ApiName, this.ApiVersions);
        }

        /// <summary>
        /// Configure dependency injection container
        /// </summary>
        /// <param name="builder">Container builder</param>
        public virtual void ConfigureContainer([NotNull] ContainerBuilder builder)
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
        public virtual void ConfigureServices([NotNull] IServiceCollection services)
        {
            var appSettings = this.GetSettings<ApplicationSettings>("ApplicationSettings");
            var seqSettings = this.GetSettings<SeqSettings>("SeqSettings");

            services.ConfigureLogging(this.Configuration, LogEventLevel.Debug, appSettings, seqSettings);

            this.ConfigureDatabaseContexts(services);

            if (this.AutoMapperAssemblies != null && this.AutoMapperAssemblies.Any())
            {
                services.AddAutoMapper(this.AutoMapperAssemblies);
            }

            this.ConfigureControllers(services);

            services.ConfigureProblemDetails();
            services.ConfigureSwagger(this.ApiName, this.ApiVersions);
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
        /// Configures local development environment
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <example>
        /// app.UseDeveloperExceptionPage();
        /// app.UseDatabaseErrorPage();
        /// </example>
        protected abstract void ConfigureLocalDevelopmentEnvironment([NotNull] IApplicationBuilder app);

        /// <summary>
        /// Configures deployed environment
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <example>app.UseHsts();</example>
        protected abstract void ConfigureDeployedEnvironment([NotNull] IApplicationBuilder app);

        /// <summary>
        /// Configures HTTPS redirection
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <example>app.UseHttpsRedirection();</example>
        protected abstract void ConfigureHttpsRedirection([NotNull] IApplicationBuilder app);

        /// <summary>
        /// Configures routing
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <example>
        /// app.UseRouting();
        /// app.UseEndpoints(endpoints => endpoints.MapControllers());
        /// </example>
        protected abstract void ConfigureRoutingAndEndpoints([NotNull] IApplicationBuilder app);

        /// <summary>
        /// Configures authentication
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <example>
        /// app.UseAuthentication();
        /// app.UseAuthorization();
        /// </example>
        protected abstract void ConfigureAuthenticationAndAuthorization([NotNull] IApplicationBuilder app);

        /// <summary>
        /// Migrates databases
        /// </summary>
        /// <param name="app">Application builder</param>
        protected abstract void MigrationDatabases([NotNull] IApplicationBuilder app);

        /// <summary>
        /// Configures <see cref="EntityManagement"/>
        /// </summary>
        /// <param name="builder">Container builder</param>
        protected abstract void ConfigureEntityManagement([NotNull] ContainerBuilder builder);

        /// <summary>
        /// Configures the database contexts used by this application
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <example>
        /// services.AddDbContextPool&lt;ApplicationDbContext&gt;(options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
        /// </example>
        protected abstract void ConfigureDatabaseContexts([NotNull] IServiceCollection services);

        /// <summary>
        /// Configures API controllers
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <example>services.AddControllers(options => options.Filters.Add(new ExceptionFilter()));</example>
        protected abstract void ConfigureControllers([NotNull] IServiceCollection services);
    }
}
