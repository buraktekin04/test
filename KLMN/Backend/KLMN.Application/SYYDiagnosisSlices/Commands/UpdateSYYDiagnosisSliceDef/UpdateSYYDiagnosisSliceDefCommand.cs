using MediatR;
 using KLMN.Application.SYYDiagnosisSlices.Responses;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.UpdateSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları: Update komutu.</summary>
 public sealed record UpdateSYYDiagnosisSliceDefCommand(Guid Id,uint Version,string Name):IRequest<SYYDiagnosisSliceDefMutationResponse>;
