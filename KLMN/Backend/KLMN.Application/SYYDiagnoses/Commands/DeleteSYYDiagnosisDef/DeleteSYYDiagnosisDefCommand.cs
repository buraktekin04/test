using MediatR;
 
 namespace KLMN.Application.SYYDiagnoses.Commands.DeleteSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F): Delete komutu.</summary>
 public sealed record DeleteSYYDiagnosisDefCommand(Guid Id,uint Version):IRequest;
