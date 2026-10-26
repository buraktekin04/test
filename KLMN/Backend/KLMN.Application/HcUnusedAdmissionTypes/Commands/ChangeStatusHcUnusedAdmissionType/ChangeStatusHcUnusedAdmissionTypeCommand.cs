using MediatR;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.ChangeStatusHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için bağımsız ChangeStatus isteği.</summary>
public sealed record ChangeStatusHcUnusedAdmissionTypeCommand(Guid Id,uint Version,bool IsActive):IRequest<HcUnusedAdmissionTypeMutationResponse>;
