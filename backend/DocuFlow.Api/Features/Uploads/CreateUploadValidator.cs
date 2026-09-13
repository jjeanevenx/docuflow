using System.Net.Mail;
using DocuFlow.Contracts.Documents;

namespace DocuFlow.Api.Features.Uploads;

public sealed class CreateUploadValidator
{
    private const int MaxDocumentsPerBatch = 20;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "application/pdf",
        "image/jpeg",
        "image/png"
    ];

    public Dictionary<string, string[]> Validate(CreateUploadRequest request)
    {
        var errors = new Dictionary<string, List<string>>();

        ValidateEmail(request.Email, errors);
        ValidateDocuments(request.Documents, errors);

        return errors.ToDictionary(
            x => x.Key,
            x => x.Value.ToArray());
    }

    private static void ValidateEmail(
        string email,
        IDictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            AddError(errors, "email", "Email is required.");
            return;
        }

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            AddError(errors, "email", "Email is invalid.");
        }
    }

    private static void ValidateDocuments(
        IReadOnlyCollection<Contracts.Uploads.UploadDocument>? documents,
        IDictionary<string, List<string>> errors)
    {
        if (documents is null || documents.Count == 0)
        {
            AddError(
                errors,
                "documents",
                "At least one document is required.");

            return;
        }

        if (documents.Count > MaxDocumentsPerBatch)
        {
            AddError(
                errors,
                "documents",
                $"A maximum of {MaxDocumentsPerBatch} documents is allowed per batch.");
        }

        var index = 0;

        foreach (var document in documents)
        {
            ValidateDocument(document, index, errors);
            index++;
        }

        ValidateDuplicates(documents, errors);
    }

    private static void ValidateDocument(
        Contracts.Uploads.UploadDocument document,
        int index,
        IDictionary<string, List<string>> errors)
    {
        var prefix = $"documents[{index}]";

        if (string.IsNullOrWhiteSpace(document.FileName))
        {
            AddError(
                errors,
                $"{prefix}.fileName",
                "File name is required.");
        }
        else if (Path.GetFileName(document.FileName) != document.FileName)
        {
            AddError(
                errors,
                $"{prefix}.fileName",
                "File name must not contain a path.");
        }

        if (string.IsNullOrWhiteSpace(document.ContentType))
        {
            AddError(
                errors,
                $"{prefix}.contentType",
                "Content type is required.");
        }
        else if (!AllowedContentTypes.Contains(document.ContentType))
        {
            AddError(
                errors,
                $"{prefix}.contentType",
                $"Content type '{document.ContentType}' is not supported.");
        }

        if (document.Size <= 0)
        {
            AddError(
                errors,
                $"{prefix}.size",
                "File size must be greater than zero.");
        }
        else if (document.Size > MaxFileSize)
        {
            AddError(
                errors,
                $"{prefix}.size",
                $"File size cannot exceed {MaxFileSize} bytes.");
        }

        if (string.IsNullOrWhiteSpace(document.Type))
        {
            AddError(
                errors,
                $"{prefix}.type",
                "Document type is required.");
        }
        else if (!Enum.TryParse<DocumentType>(
                     document.Type,
                     ignoreCase: true,
                     out _))
        {
            AddError(
                errors,
                $"{prefix}.type",
                $"Document type '{document.Type}' is invalid.");
        }

        ValidateMetadata(
            document.Metadata,
            prefix,
            errors);
    }

    private static void ValidateMetadata(
        IReadOnlyDictionary<string, string>? metadata,
        string prefix,
        IDictionary<string, List<string>> errors)
    {
        if (metadata is null)
            return;

        if (metadata.Count > 10)
        {
            AddError(
                errors,
                $"{prefix}.metadata",
                "A maximum of 10 metadata entries is allowed.");
        }

        foreach (var item in metadata)
        {
            if (string.IsNullOrWhiteSpace(item.Key))
            {
                AddError(
                    errors,
                    $"{prefix}.metadata",
                    "Metadata key cannot be empty.");

                continue;
            }

            if (item.Key.Length > 50)
            {
                AddError(
                    errors,
                    $"{prefix}.metadata.{item.Key}",
                    "Metadata key cannot exceed 50 characters.");
            }

            if (string.IsNullOrWhiteSpace(item.Value))
            {
                AddError(
                    errors,
                    $"{prefix}.metadata.{item.Key}",
                    "Metadata value cannot be empty.");
            }
            else if (item.Value.Length > 250)
            {
                AddError(
                    errors,
                    $"{prefix}.metadata.{item.Key}",
                    "Metadata value cannot exceed 250 characters.");
            }
        }
    }

    private static void ValidateDuplicates(
        IEnumerable<Contracts.Uploads.UploadDocument> documents,
        IDictionary<string, List<string>> errors)
    {
        var duplicates = documents
            .GroupBy(
                x => new
                {
                    FileName = x.FileName.ToUpperInvariant(),
                    Type = x.Type.ToUpperInvariant()
                })
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        foreach (var duplicate in duplicates)
        {
            AddError(
                errors,
                "documents",
                $"Document '{duplicate.FileName}' with type '{duplicate.Type}' is duplicated.");
        }
    }

    private static void AddError(
        IDictionary<string, List<string>> errors,
        string key,
        string message)
    {
        if (!errors.TryGetValue(key, out var values))
        {
            values = [];
            errors[key] = values;
        }

        values.Add(message);
    }
}
