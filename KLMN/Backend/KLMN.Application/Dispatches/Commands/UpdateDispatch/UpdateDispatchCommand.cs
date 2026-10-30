using KLMN.Application.Dispatches.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Commands.UpdateDispatch;
/// <summary>Sürüm denetimiyle sevk alanlarını düzenler, sevk sıra numarasını korur.</summary>
public sealed record UpdateDispatchCommand(Guid Id,uint Version,DispatchPersonInput Person,DispatchDetailsInput Dispatch) : IRequest<DispatchMutationResponse>;
