using System;
using System.Collections.Generic;
using System.Reflection;
using AspNetCoreApi.Boilerplate;
using Autofac;
using EntityManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement;
using SampleApiWebApp.Data;

namespace SampleApiWebApp
{
    public sealed class Startup : AppStartupBase
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string ApiName => "Sample API";

        protected override IList<string> ApiVersions => new List<string> { "v1" };

        protected override IEnumerable<Assembly> MediatrAssemblies => new Assembly[] { typeof(Startup).Assembly };

        protected override IList<Assembly> AutoMapperAssemblies => new List<Assembly> { typeof(Startup).Assembly };

        protected override void ConfigureLocalDevelopmentEnvironment(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
        }

        protected override void ConfigureDeployedEnvironment(IApplicationBuilder app)
        {
            app.UseHsts();
        }

        protected override void ConfigureHttpsRedirection(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
        }

        protected override void ConfigureRoutingAndEndpoints(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        protected override void ConfigureAuthenticationAndAuthorization(IApplicationBuilder app)
        {
        }

        protected override void MigrationDatabases(IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Database.Migrate();
            }
        }

        protected override void ConfigureEntityManagement(ContainerBuilder builder)
        {
            builder.RegisterModule(new EntityManagementModule<DatabaseContext>());
        }

        protected override void ConfigureDatabaseContexts(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
        }

        protected override void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add(new ExceptionFilter()));
        }
    }
}
