using Microsoft.Extensions.Logging;

namespace DreamCloud.Functions.HealthCheck;

internal static class EventIds
{
    public static readonly EventId HealthCheckProcessingBegin = new EventId(100, "HealthCheckProcessingBegin");
    public static readonly EventId HealthCheckProcessingEnd = new EventId(101, "HealthCheckProcessingEnd");

    public static readonly EventId HealthCheckBegin = new EventId(102, "HealthCheckBegin");
    public static readonly EventId HealthCheckEnd = new EventId(103, "HealthCheckEnd");
    public static readonly EventId HealthCheckError = new EventId(104, "HealthCheckError");
    public static readonly EventId HealthCheckData = new EventId(105, "HealthCheckData");
}