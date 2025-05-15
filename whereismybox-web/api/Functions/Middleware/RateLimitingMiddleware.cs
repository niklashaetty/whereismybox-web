using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Primitives;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Functions.Middleware;

public class RateLimitingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly Container _container;

    public RateLimitingMiddleware(RateLimitingMiddleWareConfiguration configuration, CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetContainer(configuration.DatabaseName, configuration.ContainerName);
    }

    public record RateLimitRecord(Guid Id, List<DateTime> Requests);

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var req = await context.GetHttpRequestDataAsync();

        if (req == null)
        {
            await next(context); // Non-HTTP trigger (like queue) — skip
            return;
        }

        UserId userId;

        try
        {
            userId = req.ParseUserId(); // Your extension method
        }
        catch
        {
            await next(context);
            return;
        }

        var now = DateTime.UtcNow;
        RateLimitRecord record;

        try
        {
            var response = await _container.ReadItemAsync<RateLimitRecord>(userId.Value.ToString(),
               PartitionKey.None);

            record = response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            record = new RateLimitRecord(userId.Value, []);
        }

        record.Requests.RemoveAll(t => (now - t).TotalMinutes > 5);

        if (record.Requests.Count >= 10)
        {
            var limitResponse = req.CreateResponse(HttpStatusCode.TooManyRequests);
            await limitResponse.WriteStringAsync("Rate limit exceeded");
            context.GetInvocationResult().Value = limitResponse;
            return;
        }

        record.Requests.Add(now);
        await _container.UpsertItemAsync(record, PartitionKey.None);

        await next(context); // Continue to function
    }
}