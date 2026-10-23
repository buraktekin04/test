using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.AdmissionTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefs;

/// <summary>Silinmemiş başvuru gruplarını sayfalı sorgular; istemcide filtre uydurmaz.</summary>
public sealed class GetHcAdmissionTypeDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHcAdmissionTypeDefsQuery, PagedResult<HcAdmissionTypeDefResponse>>
{
    /// <summary>Görüntülenen durumlar için aynı standart aktif/pasif filtresini uygular.</summary>
    public async Task<PagedResult<HcAdmissionTypeDefResponse>> Handle(
        GetHcAdmissionTypeDefsQuery request, CancellationToken ct)
    {
        var query = db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            if (int.TryParse(search, out var code))
                query = query.Where(x => x.Code == code || x.Name.ToUpper().Contains(search));
            else
                query = query.Where(x => x.Name.ToUpper().Contains(search));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x =>
        new HcAdmissionTypeDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            ForcesCommandRequired = x.ForcesCommandRequired,
            RankRequired = x.RankRequired,
            MilitaryClassRequired = x.MilitaryClassRequired,
            MilitaryUnitRequired = x.MilitaryUnitRequired,
            SenderChairRequired = x.SenderChairRequired,
            MilitaryOfficeRequired = x.MilitaryOfficeRequired,
            RegistryNumberRequired = x.RegistryNumberRequired,
            MilitarySchoolRequired = x.MilitarySchoolRequired,
            LastNominationDateRequired = x.LastNominationDateRequired,
            HeightWeightRequired = x.HeightWeightRequired,
            NthAdmissionRequired = x.NthAdmissionRequired,
            DispatchDocumentDateRequired = x.DispatchDocumentDateRequired,
            DispatchDocumentNoRequired = x.DispatchDocumentNoRequired,
            PertemApprovalRequired = x.PertemApprovalRequired,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            IsActive = x.IsActive, CreatedDate = x.CreatedDate,
            CreatedBy = x.CreatedBy, UpdatedDate = x.UpdatedDate,
            UpdatedBy = x.UpdatedBy, Version = x.Version
        }).ToListAsync(ct);
        return new PagedResult<HcAdmissionTypeDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}
