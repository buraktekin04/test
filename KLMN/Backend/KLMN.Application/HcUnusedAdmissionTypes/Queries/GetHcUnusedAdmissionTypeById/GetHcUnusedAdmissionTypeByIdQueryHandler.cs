using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;

using KLMN.Application.HcUnusedAdmissionTypes.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypeById;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedAdmissionTypeByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedAdmissionTypeByIdQuery,HcUnusedAdmissionTypeResponse>
{
 public async Task<HcUnusedAdmissionTypeResponse> Handle(GetHcUnusedAdmissionTypeByIdQuery r,CancellationToken ct)
 {
    return await db.HcUnusedAdmissionTypes.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
        .Select(x=>new HcUnusedAdmissionTypeResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.HcAdmissionTypeDefId,x.AdmissionType.Name,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new HcUnusedAdmissionTypeNotFoundException();
 }
}
