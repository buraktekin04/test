namespace KLMN.Application.SYYRules.Responses;
/// <summary>Kuralın istediği branş, yaş aralığı ve orijinal cinsiyet kodu.</summary>
public sealed record SYYRuleBranchResponse(Guid Id,string BranchName,int StartingAge,int EndingAge,int Gender,bool IsActive);
