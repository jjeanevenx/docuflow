namespace DocuFlow.Aws.DynamoDb;

public interface IDocumentQueryRepository
{
    Task<IReadOnlyCollection<DocumentEntity>> GetByBatchAsync(
        string batchId,
        CancellationToken cancellationToken);
}
