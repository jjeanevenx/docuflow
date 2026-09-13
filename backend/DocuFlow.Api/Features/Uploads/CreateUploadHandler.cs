using System.Globalization;
using DocuFlow.Api.Configuration;
using DocuFlow.Api.Services;
using DocuFlow.Aws.S3;
using DocuFlow.Contracts.Uploads;
using Microsoft.Extensions.Options;

namespace DocuFlow.Api.Features.Uploads;

public sealed class CreateUploadHandler(
    PresignedUrlService presignedUrlService,
    IOptions<AwsOptions> options)
{
    public CreateUploadResponse Handle(CreateUploadRequest request)
    {
        var batchId = Guid.NewGuid().ToString("N");

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(
            options.Value.PresignedUrlExpirationMinutes);

        var categoryCounts = request.Documents
            .GroupBy(
                document => NormalizeCategory(document.Type),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Count(),
                StringComparer.OrdinalIgnoreCase);

        var targets = request.Documents
            .Select(document => CreateTarget(
                request,
                document,
                batchId,
                categoryCounts))
            .ToArray();

        return new CreateUploadResponse(
            batchId,
            expiresAt,
            targets);
    }

    private UploadTarget CreateTarget(
        CreateUploadRequest request,
        UploadDocument document,
        string batchId,
        IReadOnlyDictionary<string, int> categoryCounts)
    {
        var documentId = Guid.NewGuid().ToString("N");
        var category = NormalizeCategory(document.Type);
        var extension = Path.GetExtension(document.FileName);

        var key = S3KeyBuilder.Raw(
            batchId,
            category,
            documentId,
            extension);

        var metadata = CreateMetadata(
            request,
            document,
            batchId,
            documentId,
            category,
            categoryCounts[category]);

        var uploadUrl = presignedUrlService.CreatePutUrl(
            key,
            document.ContentType,
            metadata);

        var headers = metadata.ToDictionary(
            item => $"x-amz-meta-{item.Key}",
            item => item.Value);

        headers["Content-Type"] = document.ContentType;

        return new UploadTarget(
            DocumentId: documentId,
            FileName: document.FileName,
            Type: category,
            UploadUrl: uploadUrl,
            Method: HttpMethod.Put.Method,
            Headers: headers);
    }

    private static Dictionary<string, string> CreateMetadata(
        CreateUploadRequest request,
        UploadDocument document,
        string batchId,
        string documentId,
        string category,
        int categoryDocumentCount)
    {
        var metadata = new Dictionary<string, string>
        {
            [UploadMetadataKeys.BatchId] = batchId,

            [UploadMetadataKeys.DocumentId] = documentId,

            [UploadMetadataKeys.DocumentType] = category,

            [UploadMetadataKeys.OriginalFileName] =
                Uri.EscapeDataString(document.FileName),

            [UploadMetadataKeys.ExpectedSize] =
                document.Size.ToString(CultureInfo.InvariantCulture),

            [UploadMetadataKeys.BatchDocumentCount] =
                request.Documents.Count.ToString(
                    CultureInfo.InvariantCulture),

            [UploadMetadataKeys.CategoryDocumentCount] =
                categoryDocumentCount.ToString(
                    CultureInfo.InvariantCulture),

            [UploadMetadataKeys.NotificationEmail] =
                request.Email
        };

        if (document.Metadata is null)
            return metadata;

        foreach (var item in document.Metadata)
        {
            metadata[
                $"{UploadMetadataKeys.CustomPrefix}{item.Key}"] =
                item.Value;
        }

        return metadata;
    }

    private static string NormalizeCategory(string type)
        => type.Trim().ToUpperInvariant();
}
