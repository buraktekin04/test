using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYRules.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYRules.Queries.GetSYYRules;
/// <summary>SYYDiagnosisDef üzerinde Eligible=true filtresi; ilişki kayıtlarını sayfa bazında toplu okur.</summary>
public sealed class GetSYYRulesQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYRulesQuery,PagedResult<SYYRuleResponse>>
{
 public async Task<PagedResult<SYYRuleResponse>> Handle(GetSYYRulesQuery r,CancellationToken ct)
 {
  var q=db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&x.Eligible==true&&(r.IncludeInactive||x.IsActive));
  if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.Id==r.SYYDiagnosisDefId.Value);
  if(!string.IsNullOrWhiteSpace(r.Search)){
   var text=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.Code.ToUpper().Contains(text)||x.Name.ToUpper().Contains(text));
  }
  var total=await q.CountAsync(ct);
  var diagnosisPage=await q.OrderBy(x=>x.Code).ThenBy(x=>x.Id)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
    .Select(x=>new {
      x.Id,x.Code,x.Name,DiagnosisTypeName=x.DiagnosisTypeDef.Name,
      SliceName=x.SliceDef!=null?x.SliceDef.Name:null,
      x.ValidFrom,x.ValidTo,x.IsActive
    }).ToListAsync(ct);
  var ids=diagnosisPage.Select(x=>x.Id).ToArray();
  if(ids.Length==0)return new PagedResult<SYYRuleResponse>{
   Items=[],TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize
  };
  // Üç sorgu yalnız bu sayfanın SYYDiagnosisDefId değerlerini okur, N+1 oluşturmaz.
  var branches=await db.SYYDiagnosisBranchDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>ids.Contains(x.SYYDiagnosisDefId)&&!x.IsDeleted&&(r.IncludeInactive||x.IsActive))
   .OrderBy(x=>x.Branch.Name).ThenBy(x=>x.Id)
   .Select(x=>new {x.SYYDiagnosisDefId,Value=new SYYRuleBranchResponse(
      x.Id,x.Branch.Name,x.StartingAge,x.EndingAge,x.Gender,x.IsActive)})
   .ToListAsync(ct);
  var procedures=await db.SYYDiagnosisProcedureDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>ids.Contains(x.SYYDiagnosisDefId)&&!x.IsDeleted&&(r.IncludeInactive||x.IsActive))
   .OrderBy(x=>x.Procedure.Name).ThenBy(x=>x.Id)
   .Select(x=>new {x.SYYDiagnosisDefId,Value=new SYYRuleProcedureResponse(
      x.Id,x.Procedure.Name,x.StartingAge,x.EndingAge,x.Gender,x.IsActive)})
   .ToListAsync(ct);
  var approvals=await db.SYYDiagnosisRequireds.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>ids.Contains(x.SYYDiagnosisDefId)&&!x.IsDeleted&&(r.IncludeInactive||x.IsActive))
   .OrderBy(x=>x.Id)
   .Select(x=>new {x.SYYDiagnosisDefId,Value=new SYYRuleApprovalResponse(
      x.Id,x.MustAll,x.MustOneOf,x.IsActive,
      x.Details.Where(d=>!d.IsDeleted&&(r.IncludeInactive||d.IsActive))
        .OrderBy(d=>d.SortOrder)
        .Select(d=>new SYYRuleApprovalDetailResponse(d.Id,d.Description,d.SortOrder)).ToList())})
   .ToListAsync(ct);
  var branchGroups=branches.GroupBy(x=>x.SYYDiagnosisDefId)
    .ToDictionary(x=>x.Key,x=>(IReadOnlyList<SYYRuleBranchResponse>)x.Select(z=>z.Value).ToList());
  var procedureGroups=procedures.GroupBy(x=>x.SYYDiagnosisDefId)
    .ToDictionary(x=>x.Key,x=>(IReadOnlyList<SYYRuleProcedureResponse>)x.Select(z=>z.Value).ToList());
  var approvalGroups=approvals.GroupBy(x=>x.SYYDiagnosisDefId)
    .ToDictionary(x=>x.Key,x=>(IReadOnlyList<SYYRuleApprovalResponse>)x.Select(z=>z.Value).ToList());
  var items=diagnosisPage.Select(x=>new SYYRuleResponse(x.Id,x.Code,x.Name,
   x.DiagnosisTypeName,x.SliceName,x.ValidFrom,x.ValidTo,x.IsActive,
   branchGroups.TryGetValue(x.Id,out var b)?b:[],
   procedureGroups.TryGetValue(x.Id,out var p)?p:[],
   approvalGroups.TryGetValue(x.Id,out var a)?a:[])).ToList();
  return new PagedResult<SYYRuleResponse>{Items=items,TotalCount=total,
    PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
