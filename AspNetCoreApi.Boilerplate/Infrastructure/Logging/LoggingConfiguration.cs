using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AspNetCoreApi.Boilerplate.Infrastructure.Logging
{
    /// <summary>
    /// Logging Configuration
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// <see cref="IServiceCollection"/> extension method to configure logging
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <param name="minimumLevel">Minimum loggging level</param>
        /// <param name="applicationSettings">Application settings</param>
        /// <param name="seqSettings">Seq settings</param>
        public static void ConfigureLogging(
            this IServiceCollection services,
            IConfiguration configuration,
            LogEventLevel minimumLevel,
            ApplicationSettings applicationSettings,
            SeqSettings seqSettings)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (applicationSettings is null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }

            if (seqSettings is null)
            {
                throw new ArgumentNullException(nameof(seqSettings));
            }

            var levelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = minimumLevel
            };

            var config = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Trace()
                .WriteTo.Console()
                .WriteTo.Seq(seqSettings.Uri, apiKey: seqSettings.Key, controlLevelSwitch: levelSwitch)
                .Enrich.WithProperty("ApplicationName", applicationSettings.Name)
                .Enrich.WithProperty("Environment", applicationSettings.Environment)
                .Enrich.WithProperty("Version", applicationSettings.Version)
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext();

            Log.Logger = config.CreateLogger();

#pragma warning disable Serilog004 // Constant MessageTemplate verifier
            Log.Logger.Information($"================= {applicationSettings.Name} Started =================");
#pragma warning restore Serilog004 // Constant MessageTemplate verifier

            services.AddLogging(builder => builder.AddSerilog(dispose: true));
        }
    }
}
