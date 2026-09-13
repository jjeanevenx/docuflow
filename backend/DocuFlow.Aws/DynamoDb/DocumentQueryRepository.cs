using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DocuFlow.Aws.DynamoDb;

public sealed class DocumentQueryRepository(
    IAmazonDynamoDB dynamoDb,
    string tableName) : IDocumentQueryRepository
{
    public async Task<IReadOnlyCollection<DocumentEntity>> GetByBatchAsync(
        string batchId,
        CancellationToken cancellationToken)
    {
        var documents = new List<DocumentEntity>();

        Dictionary<string, AttributeValue>? lastEvaluatedKey = null;

        do
        {
            var request = new QueryRequest
            {
                TableName = tableName,

                KeyConditionExpression =
                    "PartitionKey = :pk AND begins_with(SortKey, :documentPrefix)",

                ExpressionAttributeValues =
                    new Dictionary<string, AttributeValue>
                    {
                        [":pk"] = new()
                        {
                            S = $"BATCH#{batchId}"
                        },

                        [":documentPrefix"] = new()
                        {
                            S = "DOCUMENT#"
                        }
                    },

                ExclusiveStartKey = lastEvaluatedKey
            };

            var response = await dynamoDb.QueryAsync(
                request,
                cancellationToken);

            documents.AddRange(
                response.Items.Select(Map));

            lastEvaluatedKey = response.LastEvaluatedKey;
        }
        while (lastEvaluatedKey is { Count: > 0 });

        return documents;
    }

    private static DocumentEntity Map(
        Dictionary<string, AttributeValue> item)
    {
        return new DocumentEntity
        {
            PartitionKey = GetRequiredString(item, "PartitionKey"),
            SortKey = GetRequiredString(item, "SortKey"),

            DocumentId = GetRequiredString(
                item,
                "DocumentId"),

            Category = GetRequiredString(
                item,
                "Category"),

            FileName = GetRequiredString(
                item,
                "FileName"),

            S3Key = GetRequiredString(
                item,
                "S3Key"),

            ThumbnailKey = GetOptionalString(
                item,
                "ThumbnailKey"),

            ContentType = GetRequiredString(
                item,
                "ContentType"),

            Size = GetLong(
                item,
                "Size"),

            Status = GetOptionalString(
                item,
                "Status") ?? "RECEIVED",

            ReceivedAt = GetDateTimeOffset(
                item,
                "ReceivedAt"),

            Metadata = GetMetadata(item)
        };
    }

    private static string GetRequiredString(
        IReadOnlyDictionary<string, AttributeValue> item,
        string attribute)
    {
        if (!item.TryGetValue(attribute, out var value) ||
            string.IsNullOrWhiteSpace(value.S))
        {
            throw new InvalidOperationException(
                $"Required DynamoDB attribute '{attribute}' was not found.");
        }

        return value.S;
    }

    private static string? GetOptionalString(
        IReadOnlyDictionary<string, AttributeValue> item,
        string attribute)
    {
        return item.TryGetValue(attribute, out var value)
            ? value.S
            : null;
    }

    private static long GetLong(
        IReadOnlyDictionary<string, AttributeValue> item,
        string attribute)
    {
        if (!item.TryGetValue(attribute, out var value))
            return 0;

        return long.TryParse(
            value.N,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var result)
                ? result
                : 0;
    }

    private static DateTimeOffset? GetDateTimeOffset(
        IReadOnlyDictionary<string, AttributeValue> item,
        string attribute)
    {
        if (!item.TryGetValue(attribute, out var value))
            return null;

        return DateTimeOffset.TryParse(
            value.S,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var result)
                ? result
                : null;
    }

    private static IReadOnlyDictionary<string, string> GetMetadata(
        IReadOnlyDictionary<string, AttributeValue> item)
    {
        if (!item.TryGetValue("Metadata", out var value) ||
            value.M is null)
        {
            return new Dictionary<string, string>();
        }

        return value.M
            .Where(x => x.Value.S is not null)
            .ToDictionary(
                x => x.Key,
                x => x.Value.S!);
    }
}
