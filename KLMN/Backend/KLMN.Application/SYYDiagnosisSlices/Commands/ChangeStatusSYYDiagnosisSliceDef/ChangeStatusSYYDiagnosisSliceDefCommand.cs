using MediatR;
 using KLMN.Application.SYYDiagnosisSlices.Responses;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.ChangeStatusSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları: ChangeStatus komutu.</summary>
 public sealed record ChangeStatusSYYDiagnosisSliceDefCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisSliceDefMutationResponse>;
