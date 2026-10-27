namespace KLMN.Application.EquivalentProcedures.Responses;
/// <summary>Tetkik kod ve isimleriyle liste gösterimi.</summary>
public sealed record EquivalentProcedureResponse(
 Guid Id,Guid ProcedureDefId,string ProcedureCode,string ProcedureName,
 Guid EquivalentProcedureId,string EquivalentProcedureCode,string EquivalentProcedureName,
 bool IsActive,uint Version,DateTime CreatedDate);
