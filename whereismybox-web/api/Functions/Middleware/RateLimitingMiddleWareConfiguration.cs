using System;

namespace Functions.Middleware;

public class RateLimitingMiddleWareConfiguration
{
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public RateLimitingMiddleWareConfiguration(string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}