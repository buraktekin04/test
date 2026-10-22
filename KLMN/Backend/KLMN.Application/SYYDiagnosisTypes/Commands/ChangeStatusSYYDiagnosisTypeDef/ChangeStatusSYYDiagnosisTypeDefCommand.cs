using MediatR;
 using KLMN.Application.SYYDiagnosisTypes.Responses;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.ChangeStatusSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları: ChangeStatus komutu.</summary>
 public sealed record ChangeStatusSYYDiagnosisTypeDefCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisTypeDefMutationResponse>;
