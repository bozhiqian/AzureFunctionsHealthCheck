using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DreamCloud.Functions.Functions;

public class HealthCheckFunc
{
    private readonly HealthCheckService _healthCheck;
    private readonly ILogger<HealthCheckFunc> _logger;

    public HealthCheckFunc(HealthCheckService healthCheck, ILogger<HealthCheckFunc> log)
    {
        _healthCheck = healthCheck;
        _logger = log;
    }

    [FunctionName("HealthCheckFunc")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "heartbeat")] HttpRequest req)
    {
        _logger.Log(LogLevel.Information, "Received heartbeat request");

        var status = await _healthCheck.CheckHealthAsync();

        return new OkObjectResult(Enum.GetName(typeof(HealthStatus), status.Status));
    }
}