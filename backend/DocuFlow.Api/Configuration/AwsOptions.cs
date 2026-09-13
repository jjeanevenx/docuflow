namespace DocuFlow.Api.Configuration;

public sealed class AwsOptions
{
    public string ServiceUrl { get; init; } = "http://localhost:4566";
    public string Region { get; init; } = "us-east-1";
    public string DocumentsBucket { get; init; } = "docuflow-documents";
    public int PresignedUrlExpirationMinutes { get; init; } = 5;
}
