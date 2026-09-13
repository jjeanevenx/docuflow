namespace DocuFlow.Aws.DynamoDb;

public sealed class CategoryEntity
{
    public required string PK { get; init; }
    public required string SK { get; init; }
    public required string Category { get; init; }
    public int ExpectedDocuments { get; init; }
    public int ReceivedDocuments { get; set; }
    public string Status { get; set; } = "WAITING_UPLOADS";
}
