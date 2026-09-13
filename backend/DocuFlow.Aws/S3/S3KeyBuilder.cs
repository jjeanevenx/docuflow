namespace DocuFlow.Aws.S3;

public static class S3KeyBuilder
{
    public static string Raw(string batchId, string category, string documentId, string extension)
        => $"raw/{batchId}/{category}/{documentId}{extension}";

    public static string ProcessedZip(string batchId, string category)
        => $"processed/{batchId}/{category}.zip";
}
