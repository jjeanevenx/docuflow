namespace DocuFlow.Aws.DynamoDb;

public sealed class BatchEntity
{
    public required string PK { get; init; }
    public string SK { get; init; } = "BATCH";
    public required string Email { get; init; }
    public int ExpectedDocuments { get; init; }
    public int ReceivedDocuments { get; set; }
    public string Status { get; set; } = "WAITING_UPLOADS";
}
