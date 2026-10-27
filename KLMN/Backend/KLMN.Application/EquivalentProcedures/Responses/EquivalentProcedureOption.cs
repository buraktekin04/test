namespace KLMN.Application.EquivalentProcedures.Responses;
/// <summary>Aranabilir dropdown seçeneği.</summary>
public sealed record EquivalentProcedureOption(Guid Id,string Code,string Name,bool IsActive,string Label);
