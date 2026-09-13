namespace DocuFlow.Api.Features.Uploads;

public static class CreateUploadEndpoint
{
    public static IEndpointRouteBuilder MapCreateUpload(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/uploads", (CreateUploadRequest request, CreateUploadHandler handler) =>
            Results.Ok(handler.Handle(request)));

        return endpoints;
    }
}
