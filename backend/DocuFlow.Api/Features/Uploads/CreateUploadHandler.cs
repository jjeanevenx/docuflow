using DocuFlow.Api.Services;
using DocuFlow.Aws.S3;

namespace DocuFlow.Api.Features.Uploads;

public sealed class CreateUploadHandler(PresignedUrlService presignedUrlService)
{
    public CreateUploadResponse Handle(CreateUploadRequest request)
    {
        var batchId = Guid.NewGuid().ToString("N");
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(5);
        var targets = new List<UploadTarget>();

        foreach (var document in request.Documents)
        {
            var documentId = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(document.FileName);
            var category = document.Type.ToUpperInvariant();
            var key = S3KeyBuilder.Raw(batchId, category, documentId, extension);

            var metadata = new Dictionary<string, string>
            {
                ["batch-id"] = batchId,
                ["document-id"] = documentId,
                ["document-type"] = category,
                ["original-filename"] = document.FileName
            };

            if (document.Metadata is not null)
                foreach (var entry in document.Metadata)
                    metadata[$"custom-{entry.Key}"] = entry.Value;

            var url = presignedUrlService.CreatePutUrl(key, document.ContentType, metadata);
            var headers = metadata.ToDictionary(x => $"x-amz-meta-{x.Key}", x => x.Value);
            headers["Content-Type"] = document.ContentType;

            targets.Add(new UploadTarget(documentId, category, url, "PUT", headers));
        }

        return new CreateUploadResponse(batchId, expiresAt, targets);
    }
}
