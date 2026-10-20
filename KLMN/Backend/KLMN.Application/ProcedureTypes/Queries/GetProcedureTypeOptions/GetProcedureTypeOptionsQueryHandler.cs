using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeOptions;
/// <summary>Yalnız aktif türleri; düzenlemede seçili pasif türü de döndürür.</summary>
public sealed class GetProcedureTypeOptionsQueryHandler(IKLMNDbContext db):
IRequestHandler<GetProcedureTypeOptionsQuery,IReadOnlyList<ProcedureTypeOption>>
{
    public async Task<IReadOnlyList<ProcedureTypeOption>> Handle(GetProcedureTypeOptionsQuery r,CancellationToken ct)=>
      await db.ProcedureTypeDefs.IgnoreQueryFilters().AsNoTracking()
      .Where(x=>!x.IsDeleted&&(x.IsActive||x.Code==r.SelectedCode))
      .OrderBy(x=>x.Code).Select(x=>new ProcedureTypeOption(x.Code,x.Name,x.IsActive)).ToListAsync(ct);
}
