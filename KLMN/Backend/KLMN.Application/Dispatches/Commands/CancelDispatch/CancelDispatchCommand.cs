using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Commands.CancelDispatch;
/// <summary>Sevk kaydını fiziksel silmek yerine İptal (4) durumuna geçirir.</summary>
public sealed record CancelDispatchCommand(Guid Id,uint Version) : IRequest<DispatchMutationResponse>;
