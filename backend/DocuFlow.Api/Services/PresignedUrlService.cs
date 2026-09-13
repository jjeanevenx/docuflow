using Amazon.S3;
using Amazon.S3.Model;
using DocuFlow.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DocuFlow.Api.Services;

public sealed class PresignedUrlService(IAmazonS3 s3, IOptions<AwsOptions> options)
{
    public string CreatePutUrl(string key, string contentType, IReadOnlyDictionary<string, string> metadata)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Value.DocumentsBucket,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(options.Value.PresignedUrlExpirationMinutes)
        };

        foreach (var item in metadata)
            request.Metadata[item.Key] = item.Value;

        return s3.GetPreSignedURL(request);
    }
}
