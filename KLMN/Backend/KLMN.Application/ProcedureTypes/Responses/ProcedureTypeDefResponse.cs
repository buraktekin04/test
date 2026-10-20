using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ProcedureTypes.Responses;
/// <summary>Dinamik Tetkik Türü Tanımları DTO'su.</summary>
public sealed record ProcedureTypeDefResponse(Guid Id,int Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{
    /// <summary>EF Core projeksiyonu.</summary>
    public static Expression<Func<ProcedureTypeDef,ProcedureTypeDefResponse>> Projection =>
        x=>new(x.Id,x.Code,x.Name,x.IsActive,x.Version,x.CreatedDate);
}
/// <summary>Kaydetme sonrasında ID ve xmin sürümü.</summary>
public sealed record ProcedureTypeDefMutationResponse(Guid Id,uint Version);
/// <summary>Mevcut enum değerlerini taşımayı sağlayan kod ve ad seçeneği.</summary>
public sealed record ProcedureTypeOption(int Code,string Name,bool IsActive);
