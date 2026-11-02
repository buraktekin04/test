using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatchAdmissionRules;

/// <summary>Başvuru grubunun parametreleri ve kullanılmayacak muayene sebeplerini tek çağrıda sağlar.</summary>
public sealed class GetDispatchAdmissionRulesQueryHandler(IKLMNDbContext db)
    :IRequestHandler<GetDispatchAdmissionRulesQuery,DispatchAdmissionRulesResponse>
{
    public async Task<DispatchAdmissionRulesResponse> Handle(GetDispatchAdmissionRulesQuery request,CancellationToken ct)
    {
        var group=await db.HcAdmissionTypeDefs.AsNoTracking()
            .Where(x=>x.Id==request.AdmissionTypeId&&x.IsActive&&!x.IsDeleted)
            .Select(x=>new {x.ForcesCommandRequired,x.RankRequired,x.MilitaryClassRequired,x.MilitaryUnitRequired,
                x.Code,x.SenderChairRequired,x.MilitaryOfficeRequired,x.RegistryNumberRequired,x.MilitarySchoolRequired,
                x.LastNominationDateRequired,x.NthAdmissionRequired,x.DispatchDocumentDateRequired,x.DispatchDocumentNoRequired})
            .SingleOrDefaultAsync(ct)??throw new DispatchConflictException("Başvuru grubu aktif değildir.");
        var blocked=await db.ReasonForExamUnusedHcAdmissionTypes.AsNoTracking()
            .Where(x=>x.HcAdmissionTypeDefId==request.AdmissionTypeId&&x.IsActive&&!x.IsDeleted)
            .Select(x=>x.ReasonForExaminationDefId).ToListAsync(ct);
        // Eski GetReasonForExaminationListByHcAdmissionType davranışı:
        // Kod 7: tüm kuvvetler; Kod 33: 1,2,3; diğer başvuru grupları: 4 ve 5 hariç.
        var allowedForces=await db.ForcesCommandDefs.AsNoTracking()
            .Where(x=>x.IsActive&&!x.IsDeleted &&
                (group.Code==7 ||
                 (group.Code==33 ? x.Code=="1"||x.Code=="2"||x.Code=="3"
                                  : x.Code!="4"&&x.Code!="5")))
            .Select(x=>x.Id).ToListAsync(ct);
        return new DispatchAdmissionRulesResponse(group.ForcesCommandRequired,group.RankRequired,
            group.MilitaryClassRequired,group.MilitaryUnitRequired,group.SenderChairRequired,
            group.MilitaryOfficeRequired,group.RegistryNumberRequired,group.MilitarySchoolRequired,
            group.LastNominationDateRequired,group.NthAdmissionRequired,group.DispatchDocumentDateRequired,
            group.DispatchDocumentNoRequired,blocked,allowedForces);
    }
}
