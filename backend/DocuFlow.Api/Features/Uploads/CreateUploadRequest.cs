using DocuFlow.Contracts.Uploads;

namespace DocuFlow.Api.Features.Uploads;

public sealed record CreateUploadRequest(string Email, IReadOnlyCollection<UploadDocument> Documents);
