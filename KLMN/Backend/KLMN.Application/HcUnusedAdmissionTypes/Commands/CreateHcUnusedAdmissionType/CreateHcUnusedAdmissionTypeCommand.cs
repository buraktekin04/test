using MediatR;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.CreateHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için bağımsız Create isteği.</summary>
public sealed record CreateHcUnusedAdmissionTypeCommand(Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcUnusedAdmissionTypeMutationResponse>;
