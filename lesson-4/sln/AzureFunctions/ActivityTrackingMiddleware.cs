using Microsoft.Azure.Functions.Worker.Middleware;
using System.Diagnostics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace AzureFunctions;

/// <summary>
/// This is just a toy middleware to get reasonable spans from Azure Functions.
/// It is not meant to be used in production.
/// </summary>
public class ActivityTrackingMiddleware : IFunctionsWorkerMiddleware
{
    public const string ActivitySourceName = "AzureFunctionsWorker";
    private static readonly ActivitySource _activitySource = new (ActivitySourceName);

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        Activity.Current = null;
        Activity? activity;
        if (context.FunctionDefinition.InputBindings.TryGetValue("req", out var metadata) &&
            metadata.Type == "httpTrigger" &&
            await context.GetHttpRequestDataAsync() is { } requestData)
        {
            var activityContext = Propagators.DefaultTextMapPropagator.Extract(new (
                new (),
                Baggage.Current
            ), requestData.Headers, ExtractContextFromHeaderCollection);
            
            activity = _activitySource.StartActivity($"{requestData.Method.ToUpper()} {context.FunctionDefinition.Name}",
                ActivityKind.Server,
                activityContext.ActivityContext);
            
            // We should fill the span attributes according the OpenTelemetry Semantic Conventions
        }
        else
        {
            activity = _activitySource.StartActivity("Function Executed", ActivityKind.Server);
        }
        
        if (activity != null)
        {
            activity.SetTag(FunctionActivityConstants.Entrypoint, context.FunctionDefinition.EntryPoint);
            activity.SetTag(FunctionActivityConstants.Id, context.FunctionDefinition.Id);
        }

        try
        {
            await next.Invoke(context);
        }
        finally
        {
            activity?.Dispose();
        }
    }
    
    private static IEnumerable<string> ExtractContextFromHeaderCollection(HttpHeadersCollection headersCollection, string key)
    {
        return headersCollection.TryGetValues(key, out var propertyValue) ? 
            propertyValue : 
            Enumerable.Empty<string>();
    }
}



internal static class FunctionActivityConstants
{
    public const string Entrypoint = "azure.function.entrypoint";
    public const string Id = "azure.function.id";
}
