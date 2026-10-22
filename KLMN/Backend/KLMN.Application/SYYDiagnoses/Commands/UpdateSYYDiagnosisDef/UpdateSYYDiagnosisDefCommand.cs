using MediatR;
 using KLMN.Application.SYYDiagnoses.Responses;
 namespace KLMN.Application.SYYDiagnoses.Commands.UpdateSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F): Update komutu.</summary>
 public sealed record UpdateSYYDiagnosisDefCommand(Guid Id,uint Version,string Code,string Name,int SYYDiagnosisType,Guid? AttachedRecNo,bool? Eligible,int? Slice,DateTime? ValidFrom,DateTime? ValidTo):IRequest<SYYDiagnosisDefMutationResponse>;
