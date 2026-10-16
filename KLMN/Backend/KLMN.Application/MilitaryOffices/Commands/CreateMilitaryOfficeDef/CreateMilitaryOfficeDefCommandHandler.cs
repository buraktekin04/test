using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryOffices.Commands.Common;
using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.CreateMilitaryOfficeDef;

/// <summary>Şube Create işlemini ilişki ve concurrency kurallarıyla yürütür.</summary>
public sealed class CreateMilitaryOfficeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateMilitaryOfficeDefCommand, MilitaryOfficeDefMutationResponse>
{
    /// <summary>İstek doğrulandıktan sonra EF Core üzerinden işlem yapar.</summary>
    public async Task<MilitaryOfficeDefMutationResponse> Handle(CreateMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        await MilitaryOfficeDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await MilitaryOfficeDefRules.EnsureCityAsync(db, request.CityId, ct);
        var item = new MilitaryOfficeDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim(),
            CityId = request.CityId, ValidFrom = request.ValidFrom, ValidTo = request.ValidTo
        };
        await db.MilitaryOfficeDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new MilitaryOfficeDefMutationResponse(item.Id, item.Version);
    }
}
