namespace DocuFlow.Api.Features.Documents;

public static class GetBatchEndpoint
{
    public static IEndpointRouteBuilder MapGetBatch(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/uploads/{batchId}", (string batchId, GetBatchHandler handler) =>
            Results.Ok(handler.Handle(batchId)));

        return endpoints;
    }
}
