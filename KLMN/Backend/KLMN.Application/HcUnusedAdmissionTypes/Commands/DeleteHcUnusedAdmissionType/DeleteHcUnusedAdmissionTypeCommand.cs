using MediatR;

namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.DeleteHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için bağımsız Delete isteği.</summary>
public sealed record DeleteHcUnusedAdmissionTypeCommand(Guid Id,uint Version):IRequest;
