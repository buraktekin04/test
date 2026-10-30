using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.GetDispatchFile;
/// <summary>Tek dosya içeriğini sevk aidiyetiyle sorgular.</summary>
public sealed record GetDispatchFileQuery(Guid DispatchId,Guid FileId):IRequest<DispatchFileDataResponse>;
