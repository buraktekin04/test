using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Tetkik Türü Tanımları: Eski ProcedureType enum kodlarının dinamik karşılığıdır.</summary>
public sealed class ProcedureTypeDef : BaseEntity
{
    /// <summary>Eski ProcedureType enum sayısal değeri; kayıtlar arası değiştirilemez.</summary>
    public int Code { get; set; }
    /// <summary>Tetkik türünün kullanıcıya gösterilen adı.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Bu türe bağlı tetkikler.</summary>
    public ICollection<ProcedureDef> Procedures { get; set; } = new List<ProcedureDef>();
}
