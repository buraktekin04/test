using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryOffices.Commands.Common;
using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.ChangeStatusMilitaryOfficeDef;

/// <summary>Şube ChangeStatus işlemini ilişki ve concurrency kurallarıyla yürütür.</summary>
public sealed class ChangeStatusMilitaryOfficeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusMilitaryOfficeDefCommand, MilitaryOfficeDefMutationResponse>
{
    /// <summary>İstek doğrulandıktan sonra EF Core üzerinden işlem yapar.</summary>
    public async Task<MilitaryOfficeDefMutationResponse> Handle(ChangeStatusMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryOfficeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await MilitaryOfficeDefRules.EnsureCityAsync(db, item.CityId, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new MilitaryOfficeDefMutationResponse(item.Id, item.Version);
    }
}
