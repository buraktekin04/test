using MediatR;
 
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.DeleteSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları: Delete komutu.</summary>
 public sealed record DeleteSYYDiagnosisTypeDefCommand(Guid Id,uint Version):IRequest;
