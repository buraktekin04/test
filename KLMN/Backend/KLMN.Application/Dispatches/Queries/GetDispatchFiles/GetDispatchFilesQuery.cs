using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.GetDispatchFiles;
/// <summary>Sevke bağlı belge meta verilerini döndürür, ham içerik göndermez.</summary>
public sealed record GetDispatchFilesQuery(Guid DispatchId):IRequest<IReadOnlyList<DispatchFileResponse>>;
