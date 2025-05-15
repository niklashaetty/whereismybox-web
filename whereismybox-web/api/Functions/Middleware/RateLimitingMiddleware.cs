using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;

namespace Functions.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Container _container;

    public RateLimitingMiddleware(RequestDelegate next, RateLimitingMiddleWareConfiguration configuration, CosmosClient cosmosClient)
    {
        _next = next;
        _container = cosmosClient.GetContainer(configuration.DatabaseName, configuration.ContainerName);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("Calling RateLimitingMiddleware");
        await _next(context);
        return;
        
        var req = context.Request;
        ExternalUser externalUser;
        try
        {
            externalUser = req.ParseExternalUser();
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid user info");
            return;
        }

        // Rate limit logic (Cosmos)
        var now = DateTime.UtcNow;
        RateLimitRecord record;

        try
        {
            var response = await _container.ReadItemAsync<RateLimitRecord>(externalUser.ExternalUserId.Value, PartitionKey.None);
            record = response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            record = new RateLimitRecord(externalUser.ExternalUserId.Value, []);
        }

        record.Requests.RemoveAll(t => (now - t).TotalMinutes > 5);

        if (record.Requests.Count >= 10)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        record.Requests.Add(now);
        var res = await _container.UpsertItemAsync(record, PartitionKey.None);

        await _next(context);
    }

    public record RateLimitRecord(string ExternalUserId, List<DateTime> Requests);
}