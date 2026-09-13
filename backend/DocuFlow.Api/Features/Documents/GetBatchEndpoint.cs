namespace DocuFlow.Api.Features.Documents;

public static class GetBatchEndpoint
{
    public static IEndpointRouteBuilder MapGetBatch(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/batches/{batchId}/documents",
            async (
                string batchId,
                GetBatchHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(
                    batchId,
                    cancellationToken);

                return Results.Ok(response);
            });

        return endpoints;
    }
}
