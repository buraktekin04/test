using MediatR;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.UpdateHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için bağımsız Update isteği.</summary>
public sealed record UpdateHcUnusedAdmissionTypeCommand(Guid Id,uint Version,Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcUnusedAdmissionTypeMutationResponse>;
