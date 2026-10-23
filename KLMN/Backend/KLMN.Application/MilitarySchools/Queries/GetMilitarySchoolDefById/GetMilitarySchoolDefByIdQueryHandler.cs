using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Exceptions;
using KLMN.Application.MilitarySchools.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefById;

/// <summary>Silinmemiş kaydı detay DTO'su olarak getirir.</summary>
public sealed class GetMilitarySchoolDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitarySchoolDefByIdQuery, MilitarySchoolDefResponse>
{
    /// <summary>Kimliğe göre okul kaydını döndürür; bulunamazsa 404 üretilir.</summary>
    public async Task<MilitarySchoolDefResponse> Handle(GetMilitarySchoolDefByIdQuery request, CancellationToken ct) =>
        await db.MilitarySchoolDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new MilitarySchoolDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            CityId = x.CityId, CityName = x.City == null ? null : x.City.Name,
            ForcesCommandDefId = x.ForcesCommandDefId,
            ForcesCommandName = x.ForcesCommand == null ? null : x.ForcesCommand.Name,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            IsActive = x.IsActive, CreatedDate = x.CreatedDate,
            CreatedBy = x.CreatedBy, UpdatedDate = x.UpdatedDate,
            UpdatedBy = x.UpdatedBy, Version = x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new MilitarySchoolNotFoundException();
}
