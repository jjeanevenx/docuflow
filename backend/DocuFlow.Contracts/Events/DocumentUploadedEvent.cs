namespace DocuFlow.Contracts.Events;

public sealed record DocumentUploadedEvent(
    string BatchId,
    string DocumentId,
    string Category,
    string Bucket,
    string Key);
