namespace KLMN.Application.HcDecisions.Common;
/// <summary>Kurul kararı ve referans kayıtları için seçim öğesi.</summary>
public sealed record HcDecisionOption(Guid Id,string Label,bool IsActive);
