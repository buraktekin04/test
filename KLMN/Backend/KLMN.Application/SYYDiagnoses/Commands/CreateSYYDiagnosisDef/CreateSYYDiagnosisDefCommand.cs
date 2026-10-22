using MediatR;
 using KLMN.Application.SYYDiagnoses.Responses;
 namespace KLMN.Application.SYYDiagnoses.Commands.CreateSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F): Create komutu.</summary>
 public sealed record CreateSYYDiagnosisDefCommand(string Code,string Name,int SYYDiagnosisType,Guid? AttachedRecNo,bool? Eligible,int? Slice,DateTime? ValidFrom,DateTime? ValidTo):IRequest<SYYDiagnosisDefMutationResponse>;
