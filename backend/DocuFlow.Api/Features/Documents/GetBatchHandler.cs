using DocuFlow.Aws.DynamoDb;

namespace DocuFlow.Api.Features.Documents;

public sealed class GetBatchHandler(
    IDocumentQueryRepository repository)
{
    public async Task<GetBatchDocumentsResponse> HandleAsync(
        string batchId,
        CancellationToken cancellationToken)
    {
        var documents = await repository.GetByBatchAsync(
            batchId,
            cancellationToken);

        var response = documents
            .Select(document =>
                new DocumentResponse(
                    DocumentId: document.DocumentId,
                    Type: document.Category,
                    FileName: document.FileName,
                    ContentType: document.ContentType,
                    Size: document.Size,
                    Status: document.Status,
                    ReceivedAt: document.ReceivedAt,
                    Metadata: document.Metadata))
            .ToArray();

        return new GetBatchDocumentsResponse(
            batchId,
            response);
    }
}
