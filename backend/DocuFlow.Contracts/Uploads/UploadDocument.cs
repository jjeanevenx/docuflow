namespace DocuFlow.Contracts.Uploads;

public sealed record UploadDocument(
    string FileName,
    string ContentType,
    long Size,
    string Type,
    IReadOnlyDictionary<string, string>? Metadata);
