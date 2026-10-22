using MediatR;
 using KLMN.Application.SYYDiagnosisTypes.Responses;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.CreateSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları: Create komutu.</summary>
 public sealed record CreateSYYDiagnosisTypeDefCommand(int Code,string Name):IRequest<SYYDiagnosisTypeDefMutationResponse>;
