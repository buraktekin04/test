using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Exceptions;
using KLMN.Application.MilitaryClasses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefById;

/// <summary>Silinmemiş sınıf kaydını liste DTO'su biçiminde getirir.</summary>
public sealed class GetMilitaryClassDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassDefByIdQuery, MilitaryClassDefResponse>
{
    /// <summary>Bulunmayan kimliği 404 problem yanıtına dönüştürülmek üzere bildirir.</summary>
    public async Task<MilitaryClassDefResponse> Handle(GetMilitaryClassDefByIdQuery request, CancellationToken ct) =>
        await db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new MilitaryClassDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            ShortName = x.ShortName, ExternalCode = x.ExternalCode,
            IsActive = x.IsActive,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy,
            Version = x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new MilitaryClassNotFoundException();
}
