using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Eski EquivalentProcedureDef: iki farklı ProcedureDef arasında simetrik eşdeğerlik.</summary>
public sealed class EquivalentProcedureDef:BaseEntity
{
 /// <summary>Guid karşılaştırmasıyla kanonik sıralanan ilk tetkik.</summary>
 public Guid ProcedureDefId {get;set;}
 public ProcedureDef Procedure {get;set;}=null!;
 /// <summary>Guid karşılaştırmasıyla kanonik sıralanan eşdeğer tetkik.</summary>
 public Guid EquivalentProcedureId {get;set;}
 public ProcedureDef EquivalentProcedure {get;set;}=null!;
}
