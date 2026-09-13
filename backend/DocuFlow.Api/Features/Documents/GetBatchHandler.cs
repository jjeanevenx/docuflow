namespace DocuFlow.Api.Features.Documents;

public sealed class GetBatchHandler
{

    public GetBachResponse Handle(string batchId)
    {
        // TODO: consultar manifesto e status do lote no DynamoDB.
        return new GetBachResponse(batchId, "TODO");
    }
}
