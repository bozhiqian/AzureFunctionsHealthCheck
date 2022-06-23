using DreamCloud.Functions.HealthCheck;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DreamCloud.Functions.Infrastructure;

public static class FunctionsAppExtensionMethods
{
    public static IConfigurationBuilder AddAppSettingsJson(this IConfigurationBuilder builder, FunctionsHostBuilderContext context)
    {
        builder.AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false);
        builder.AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false);
        return builder;
    }

    /// <summary>
    /// Adds the <see cref="HealthCheckService"/> to the container, using the provided delegate to register
    /// health checks.
    /// </summary>
    /// <remarks>
    /// This operation is idempotent - multiple invocations will still only result in a single
    /// <see cref="HealthCheckService"/> instance in the <see cref="IServiceCollection"/>. It can be invoked
    /// multiple times in order to get access to the <see cref="IHealthChecksBuilder"/> in multiple places.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="HealthCheckService"/> to.</param>
    /// <returns>An instance of <see cref="IHealthChecksBuilder"/> from which health checks can be registered.</returns>
    public static IHealthChecksBuilder AddFunctionHealthChecks(this IServiceCollection services)
    {
        services.TryAddSingleton<HealthCheckService, DefaultHealthCheckService>();
        return new HealthChecksBuilder(services);
    }
}