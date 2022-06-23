using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DreamCloud.Functions.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DreamCloud.Functions.HealthCheck;

internal class DefaultHealthCheckService : HealthCheckService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<HealthCheckServiceOptions> _options;
    private readonly ILogger<DefaultHealthCheckService> _logger;

    public DefaultHealthCheckService(
        IServiceScopeFactory scopeFactory,
        IOptions<HealthCheckServiceOptions> options,
        ILogger<DefaultHealthCheckService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ValidateRegistrations(_options.Value.Registrations);
    }
    public override async Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool> predicate,
        CancellationToken cancellationToken = default)
    {
        var registrations = _options.Value.Registrations;

        using var scope = _scopeFactory.CreateScope();
        var context = new HealthCheckContext();
        var entries = new Dictionary<string, HealthReportEntry>(StringComparer.OrdinalIgnoreCase);

        var totalTime = ValueStopwatch.StartNew();
        _logger.HealthCheckProcessingBegin();

        foreach (var registration in registrations)
        {
            if (predicate != null && !predicate(registration))
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var healthCheck = registration.Factory(scope.ServiceProvider);

            var stopwatch = ValueStopwatch.StartNew();
            context.Registration = registration;

            _logger.HealthCheckBegin(registration);

            HealthReportEntry entry;
            try
            {
                var result = await healthCheck.CheckHealthAsync(context, cancellationToken);
                var duration = stopwatch.Elapsed;

                entry = new HealthReportEntry(
                    status: result.Status,
                    description: result.Description,
                    duration: duration,
                    exception: result.Exception,
                    data: result.Data);

                _logger.HealthCheckEnd(registration, entry, duration);
                _logger.HealthCheckData(registration, entry);
            }

            // Allow cancellation to propagate.
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var duration = stopwatch.Elapsed;
                entry = new HealthReportEntry(
                    status: HealthStatus.Unhealthy,
                    description: ex.Message,
                    duration: duration,
                    exception: ex,
                    data: null);

                _logger.HealthCheckError(registration, ex, duration);
            }

            entries[registration.Name] = entry;
        }

        var totalElapsedTime = totalTime.Elapsed;
        var report = new HealthReport(entries, totalElapsedTime);
        _logger.HealthCheckProcessingEnd(report.Status, totalElapsedTime);
        return report;
    }

    private static void ValidateRegistrations(IEnumerable<HealthCheckRegistration> registrations)
    {
        // Scan the list for duplicate names to provide a better error if there are duplicates.
        var duplicateNames = registrations
            .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Count > 0)
        {
            throw new ArgumentException($"Duplicate health checks were registered with the name(s): {string.Join(", ", duplicateNames)}", nameof(registrations));
        }
    }
}

