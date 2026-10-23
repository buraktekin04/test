using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Hospitals.Exceptions;
using KLMN.Application.Hospitals.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Hospitals.Queries.GetHospitalDefById;

/// <summary>Karar detayını seçilen GUID ile sorgular.</summary>
public sealed class GetHospitalDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalDefByIdQuery, HospitalDefResponse>
{
    /// <summary>Silinmiş veya olmayan sağlık tesisiı 404 olarak ele alır.</summary>
    public async Task<HospitalDefResponse> Handle(GetHospitalDefByIdQuery request, CancellationToken ct) =>
        await db.HospitalDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new HospitalDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            CityId = x.CityId, CityName = x.City == null ? null : x.City.Name,
            RefereeHospitalId = x.RefereeHospitalId,
            RefereeHospitalName = x.RefereeHospital == null ? null : x.RefereeHospital.Name,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new HospitalNotFoundException();
}
