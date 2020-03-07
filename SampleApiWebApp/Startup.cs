using System.Collections.Generic;
using System.Reflection;
using AspNetCoreApi.Boilerplate;
using Autofac;
using EntityManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        protected override void MigrationDatabases(IApplicationBuilder app)
        {
            MigrationDatabase<DatabaseContext>(app);
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

        protected override void ConfigureHealthChecks(IHealthChecksBuilder builder)
        {
            builder.AddCheck<DatabaseHealthCheck<DatabaseContext>>(nameof(DatabaseContext));
        }
    }
}
