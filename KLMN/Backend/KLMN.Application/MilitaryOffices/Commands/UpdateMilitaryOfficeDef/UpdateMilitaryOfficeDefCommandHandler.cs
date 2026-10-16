using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryOffices.Commands.Common;
using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.UpdateMilitaryOfficeDef;

/// <summary>Şube Update işlemini ilişki ve concurrency kurallarıyla yürütür.</summary>
public sealed class UpdateMilitaryOfficeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateMilitaryOfficeDefCommand, MilitaryOfficeDefMutationResponse>
{
    /// <summary>İstek doğrulandıktan sonra EF Core üzerinden işlem yapar.</summary>
    public async Task<MilitaryOfficeDefMutationResponse> Handle(UpdateMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryOfficeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await MilitaryOfficeDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        await MilitaryOfficeDefRules.EnsureCityAsync(db, request.CityId, ct, item.CityId);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.CityId = request.CityId;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new MilitaryOfficeDefMutationResponse(item.Id, item.Version);
    }
}
