using MediatR;
using KLMN.Application.ICD10Defs.Responses;
namespace KLMN.Application.ICD10Defs.Commands.ChangeStatusICD10Def;
/// <summary>ICD10 Tanımları: ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusICD10DefCommand(Guid Id,uint Version,bool IsActive):IRequest<ICD10DefMutationResponse>;
