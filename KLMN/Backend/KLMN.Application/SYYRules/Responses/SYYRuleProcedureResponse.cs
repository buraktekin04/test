namespace KLMN.Application.SYYRules.Responses;
/// <summary>Kuralın istediği tetkik, yaş aralığı ve orijinal cinsiyet kodu.</summary>
public sealed record SYYRuleProcedureResponse(Guid Id,string ProcedureName,int StartingAge,int EndingAge,int Gender,bool IsActive);
