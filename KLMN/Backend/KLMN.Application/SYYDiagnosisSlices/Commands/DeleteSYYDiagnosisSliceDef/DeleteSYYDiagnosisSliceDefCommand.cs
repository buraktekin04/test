using MediatR;
 
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.DeleteSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları: Delete komutu.</summary>
 public sealed record DeleteSYYDiagnosisSliceDefCommand(Guid Id,uint Version):IRequest;
