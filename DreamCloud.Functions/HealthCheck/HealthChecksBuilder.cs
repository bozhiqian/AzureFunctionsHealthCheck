﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace DreamCloud.Functions.HealthCheck;

internal class HealthChecksBuilder : IHealthChecksBuilder
{
    public HealthChecksBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public IHealthChecksBuilder Add(HealthCheckRegistration registration)
    {
        if (registration == null)
        {
            throw new ArgumentNullException(nameof(registration));
        }

        Services.Configure<HealthCheckServiceOptions>(options =>
        {
            options.Registrations.Add(registration);
        });

        return this;
    }
}