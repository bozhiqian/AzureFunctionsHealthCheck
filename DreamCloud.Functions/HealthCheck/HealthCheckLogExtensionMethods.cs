using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace DreamCloud.Functions.HealthCheck;

internal static class HealthCheckLogExtensions
{
    private static string HealthCheckEndText(string healthCheckName, double elapsedMilliseconds, HealthStatus healthStatus, string healthCheckDescription)
        => $"Health check {healthCheckName} completed after {elapsedMilliseconds}ms with status {healthStatus} and '{healthCheckDescription}'";
    
    public static void HealthCheckProcessingBegin(this ILogger logger)
    {
        logger.Log(LogLevel.Debug,
            EventIds.HealthCheckProcessingBegin,
            "Running health checks");
    }

    public static void HealthCheckProcessingEnd(this ILogger logger, HealthStatus status, TimeSpan duration)
    {
        logger.Log(LogLevel.Debug,
            EventIds.HealthCheckProcessingEnd,
            $"Health check processing completed after {duration.TotalMilliseconds}ms with combined status {status}");
    }

    public static void HealthCheckBegin(this ILogger logger, HealthCheckRegistration registration)
    {
        logger.Log(LogLevel.Debug,
            EventIds.HealthCheckBegin,
            $"Running health check {registration.Name}");
    }

    public static void HealthCheckEnd(this ILogger logger, HealthCheckRegistration registration, HealthReportEntry entry, TimeSpan duration)
    {
        switch (entry.Status)
        {
            case HealthStatus.Healthy:
                logger.Log(LogLevel.Debug,
                    EventIds.HealthCheckEnd,
                    HealthCheckEndText(registration.Name, duration.TotalMilliseconds, entry.Status, entry.Description));
                break;

            case HealthStatus.Degraded:
                logger.Log(LogLevel.Warning,
                    EventIds.HealthCheckEnd,
                    HealthCheckEndText(registration.Name, duration.TotalMilliseconds, entry.Status, entry.Description));
                break;

            case HealthStatus.Unhealthy:
                logger.Log(LogLevel.Error,
                    EventIds.HealthCheckEnd,
                    HealthCheckEndText(registration.Name, duration.TotalMilliseconds, entry.Status, entry.Description));
                break;
        }
    }

    public static void HealthCheckError(this ILogger logger, HealthCheckRegistration registration, Exception exception, TimeSpan duration)
    {
        logger.Log(LogLevel.Error,
            EventIds.HealthCheckError,
            $"Health check {registration.Name} threw an unhandled exception after {duration.TotalMilliseconds}ms", exception);
    }

    public static void HealthCheckData(this ILogger logger, HealthCheckRegistration registration, HealthReportEntry entry)
    {
        if (entry.Data.Count > 0 && logger.IsEnabled(LogLevel.Debug))
        {
            logger.Log(
                LogLevel.Debug,
                EventIds.HealthCheckData,
                new HealthCheckDataLogValue(registration.Name, entry.Data),
                null,
                (state, ex) => state.ToString());
        }
    }
}