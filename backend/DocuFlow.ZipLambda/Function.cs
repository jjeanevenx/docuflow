using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DocuFlow.ZipLambda;

public sealed class Function
{
    public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
    {
        foreach (var message in input.Records)
        {
            context.Logger.LogInformation($"Zip message: {message.Body}");
            // TODO: verificar categoria completa, adquirir lock no DynamoDB, gerar ZIP e salvar no S3.
            await Task.CompletedTask;
        }
    }
}
