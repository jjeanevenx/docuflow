namespace DocuFlow.Api.Features.Documents;

public sealed record GetBatchDocumentsResponse(
    string BatchId,
    IReadOnlyCollection<DocumentResponse> Documents);

public sealed record DocumentResponse(
    string DocumentId,
    string Type,
    string FileName,
    string ContentType,
    long Size,
    string Status,
    DateTimeOffset? ReceivedAt,
    IReadOnlyDictionary<string, string> Metadata);
