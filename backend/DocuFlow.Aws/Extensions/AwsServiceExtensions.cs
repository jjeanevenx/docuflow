using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SimpleEmailV2;
using DocuFlow.Aws.DynamoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocuFlow.Aws.Extensions;

public static class AwsServiceExtensions
{
    public static IServiceCollection AddDocuFlowAws(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceUrl = configuration["Aws:ServiceUrl"] ?? "http://localhost:4566";
        var region = configuration["Aws:Region"] ?? "us-east-1";

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
            new Amazon.Runtime.BasicAWSCredentials("test", "test"),
            new AmazonS3Config { ServiceURL = serviceUrl, ForcePathStyle = true, AuthenticationRegion = region }));

        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(
            new Amazon.Runtime.BasicAWSCredentials("test", "test"),
            new AmazonDynamoDBConfig { ServiceURL = serviceUrl, AuthenticationRegion = region }));

        var inventoryTable =
    configuration["Aws:InventoryTable"]
    ?? "docuflow-inventory";

        services.AddSingleton<IDocumentQueryRepository>(provider =>
            new DocumentQueryRepository(
                provider.GetRequiredService<IAmazonDynamoDB>(),
                inventoryTable));

        services.AddSingleton<IAmazonSimpleEmailServiceV2>(_ => new AmazonSimpleEmailServiceV2Client(
            new Amazon.Runtime.BasicAWSCredentials("test", "test"),
            new AmazonSimpleEmailServiceV2Config { ServiceURL = serviceUrl, AuthenticationRegion = region }));

        return services;
    }
}
