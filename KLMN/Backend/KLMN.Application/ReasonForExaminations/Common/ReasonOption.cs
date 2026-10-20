namespace KLMN.Application.ReasonForExaminations.Common;
/// <summary>Muayene modüllerinin dropdown seçenekleri; diğer modülün View iznini gerektirmez.</summary>
public sealed record ReasonOption(Guid Id, string Label, bool IsActive);
/// <summary>Muayene ve referans kayıtlarını tek çağrıda sağlar.</summary>
public sealed record ReasonRelationOptions(IReadOnlyList<ReasonOption> Reasons, IReadOnlyList<ReasonOption> References);
