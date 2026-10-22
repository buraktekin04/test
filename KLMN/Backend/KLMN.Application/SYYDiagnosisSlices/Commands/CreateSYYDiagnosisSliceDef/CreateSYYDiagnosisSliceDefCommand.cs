using MediatR;
 using KLMN.Application.SYYDiagnosisSlices.Responses;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.CreateSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları: Create komutu.</summary>
 public sealed record CreateSYYDiagnosisSliceDefCommand(int Code,string Name):IRequest<SYYDiagnosisSliceDefMutationResponse>;
