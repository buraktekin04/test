using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AdmissionTypes.Exceptions;
using KLMN.Application.AdmissionTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefById;

/// <summary>Silinmemiş başvuru grubu detayını salt okunur döndürür.</summary>
public sealed class GetHcAdmissionTypeDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHcAdmissionTypeDefByIdQuery, HcAdmissionTypeDefResponse>
{
    /// <summary>Mevcut değilse 404, varsa proje edilen yanıtı verir.</summary>
    public async Task<HcAdmissionTypeDefResponse> Handle(
        GetHcAdmissionTypeDefByIdQuery request, CancellationToken ct) =>
        await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
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
        }).SingleOrDefaultAsync(ct)
            ?? throw new AdmissionTypeNotFoundException();
}
