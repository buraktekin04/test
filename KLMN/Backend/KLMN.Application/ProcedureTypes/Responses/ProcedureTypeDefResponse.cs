using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ProcedureTypes.Responses;
/// <summary>Dinamik Tetkik Türü Tanımları DTO'su.</summary>
public sealed record ProcedureTypeDefResponse(Guid Id,int Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{
    /// <summary>EF Core projeksiyonu.</summary>

}
