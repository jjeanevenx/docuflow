namespace DocuFlow.Aws.DynamoDb;

public sealed class DocumentEntity
{
    public required string PK { get; init; }
    public required string SK { get; init; }
    public required string DocumentId { get; init; }
    public required string Category { get; init; }
    public required string FileName { get; init; }
    public required string S3Key { get; init; }
    public required string ContentType { get; init; }
    public long ExpectedSize { get; init; }
    public string Status { get; set; } = "PENDING_UPLOAD";
}
