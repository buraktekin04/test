namespace KLMN.Application.HcDecisions.Common;
/// <summary>İlişki ekranının ana karar ve ikinci referans seçenekleri.</summary>
public sealed record HcDecisionRelationOptions(IReadOnlyList<HcDecisionOption> Decisions,IReadOnlyList<HcDecisionOption> References);
