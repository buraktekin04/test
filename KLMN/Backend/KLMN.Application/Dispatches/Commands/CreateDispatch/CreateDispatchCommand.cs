using KLMN.Application.Dispatches.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Commands.CreateDispatch;
/// <summary>Yeni sevk ve kişiyi tek transaction içinde oluşturan CQRS komutu.</summary>
public sealed record CreateDispatchCommand(DispatchPersonInput Person,DispatchDetailsInput Dispatch) : IRequest<DispatchMutationResponse>;
