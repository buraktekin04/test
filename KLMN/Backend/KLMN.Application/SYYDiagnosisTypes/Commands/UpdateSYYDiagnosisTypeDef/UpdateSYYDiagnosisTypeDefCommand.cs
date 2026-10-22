using MediatR;
 using KLMN.Application.SYYDiagnosisTypes.Responses;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.UpdateSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları: Update komutu.</summary>
 public sealed record UpdateSYYDiagnosisTypeDefCommand(Guid Id,uint Version,string Name):IRequest<SYYDiagnosisTypeDefMutationResponse>;
