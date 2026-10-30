using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.GetDispatchById;
/// <summary>Düzenleme ekranının tam sevk/kişi verilerini döndürür.</summary>
public sealed record GetDispatchByIdQuery(Guid Id):IRequest<DispatchResponse>;
