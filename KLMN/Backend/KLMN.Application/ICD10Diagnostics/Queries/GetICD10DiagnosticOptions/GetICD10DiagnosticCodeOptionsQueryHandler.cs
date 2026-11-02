using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticOptions;

/// <summary>
/// Büyük ICD10 sözlüklerinde bütün tanımları dialog açılırken transfer etmez.
/// İlk 100 tanım veya kod/ad aramasındaki ilk 100 sonuç döner; seçili ID korunur.
/// </summary>
public sealed class GetICD10DiagnosticCodeOptionsQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetICD10DiagnosticCodeOptionsQuery,IReadOnlyList<ICD10Option>>
{
 public async Task<IReadOnlyList<ICD10Option>> Handle(
  GetICD10DiagnosticCodeOptionsQuery request,CancellationToken ct)
 {
  var q=db.ICD10Defs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==request.SelectedICD10DefId));

  var term=request.Search?.Trim();
  if(!string.IsNullOrWhiteSpace(term))
  {
   // LIKE jokerleri kullanıcıdan geliyorsa gerçek karakter olarak aranmalı.
   if(term.Length>100)term=term[..100];
   var escaped=term.Replace(@"\",@"\\").Replace("%",@"\%").Replace("_",@"\_");
   var pattern="%"+escaped+"%";
   q=q.Where(x=>x.Id==request.SelectedICD10DefId||
    EF.Functions.ILike(x.Code,pattern,@"\")||
    EF.Functions.ILike(x.Name,pattern,@"\"));
  }

  return await q.OrderByDescending(x=>x.Id==request.SelectedICD10DefId)
   .ThenBy(x=>x.Code).Take(100)
   .Select(x=>new ICD10Option(x.Id,x.Code+" — "+x.Name,x.IsActive))
   .ToListAsync(ct);
 }
}
