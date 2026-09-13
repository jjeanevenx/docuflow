using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DocuFlow.InventoryLambda;

public sealed class Function
{
    public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
    {
        foreach (var message in input.Records)
        {
            context.Logger.LogInformation($"Inventory message: {message.Body}");
            // TODO: interpretar envelope SNS/S3, validar objeto, atualizar DynamoDB e notificar via SES.
            await Task.CompletedTask;
        }
    }
}
