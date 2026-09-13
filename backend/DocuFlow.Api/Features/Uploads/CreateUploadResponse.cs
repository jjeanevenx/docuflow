namespace DocuFlow.Api.Features.Uploads;

public sealed record CreateUploadResponse(string BatchId, DateTimeOffset ExpiresAt, IReadOnlyCollection<UploadTarget> Documents);

public sealed record UploadTarget(
    string DocumentId,
    string Type,
    string UploadUrl,
    string Method,
    IReadOnlyDictionary<string, string> Headers);
