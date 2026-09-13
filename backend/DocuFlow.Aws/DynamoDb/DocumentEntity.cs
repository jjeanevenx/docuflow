namespace DocuFlow.Aws.DynamoDb;

public sealed class DocumentEntity
{
    public required string PartitionKey { get; init; }
    public required string SortKey { get; init; }

    public required string DocumentId { get; init; }
    public required string Category { get; init; }
    public required string FileName { get; init; }

    public required string S3Key { get; init; }
    public string? ThumbnailKey { get; init; }

    public required string ContentType { get; init; }

    public long Size { get; init; }

    public string Status { get; init; } = "RECEIVED";

    public DateTimeOffset? ReceivedAt { get; init; }

    public IReadOnlyDictionary<string, string> Metadata { get; init; }
        = new Dictionary<string, string>();
}
