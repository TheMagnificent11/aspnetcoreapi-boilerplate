using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AspNetCoreApi.Boilerplate.Infrastructure
{
    /// <summary>
    /// Swagger Configuration
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configures Swagger and Swagger UI
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="apiName">API name</param>
        /// <param name="apiVersions">API versions</param>
        public static void ConfigureSwagger(this IApplicationBuilder app, string apiName, IList<string> apiVersions)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                apiVersions
                    .ToList()
                    .ForEach(version => c.SwaggerEndpoint($"/swagger/{version}/swagger.json", apiName));
            });
        }

        /// <summary>
        /// Configures Swagger
        /// </summary>
        /// <param name="services">SErvices collection</param>
        /// <param name="apiName">API name</param>
        /// <param name="apiVersions">API versions</param>
        /// <param name="securitySchemaName">Security schema name/type</param>
        /// <param name="securityScheme">Security scheme</param>
        public static void ConfigureSwagger(
            this IServiceCollection services,
            string apiName,
            IList<string> apiVersions,
            string securitySchemaName = null,
            OpenApiSecurityScheme securityScheme = null)
        {
            services.AddSwaggerGen(c =>
            {
                apiVersions
                    .ToList()
                    .ForEach(version =>
                    {
                        c.SwaggerDoc(version, new OpenApiInfo { Title = apiName, Version = version });

                        if (securityScheme == null || string.IsNullOrWhiteSpace(securitySchemaName))
                            return;

                        c.AddSecurityDefinition(securitySchemaName, securityScheme);
                    });
            });
        }
    }
}
