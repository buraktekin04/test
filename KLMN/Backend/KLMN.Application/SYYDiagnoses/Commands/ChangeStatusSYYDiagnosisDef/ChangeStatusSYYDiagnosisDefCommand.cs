using MediatR;
 using KLMN.Application.SYYDiagnoses.Responses;
 namespace KLMN.Application.SYYDiagnoses.Commands.ChangeStatusSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F): ChangeStatus komutu.</summary>
 public sealed record ChangeStatusSYYDiagnosisDefCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisDefMutationResponse>;
