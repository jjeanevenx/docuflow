namespace DocuFlow.Api.Features.Uploads;

public static class CreateUploadEndpoint
{
    public static IEndpointRouteBuilder MapCreateUpload(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/uploads", 
            (CreateUploadRequest request,
            CreateUploadValidator validator,
            CreateUploadHandler handler) =>
            {
                var errors = validator.Validate(request);

                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                return Results.Ok(handler.Handle(request));
            });

        return endpoints;
    }
}
