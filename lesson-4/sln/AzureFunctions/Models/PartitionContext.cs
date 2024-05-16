namespace AzureFunctions.Models;

// Workaround for the broken binding: https://github.com/Azure/azure-functions-dotnet-worker/issues/2293
public record PartitionContext(string? PartitionId, string? ConsumerGroup, string? EventHubName, string? FullyQualifiedNamespace);
