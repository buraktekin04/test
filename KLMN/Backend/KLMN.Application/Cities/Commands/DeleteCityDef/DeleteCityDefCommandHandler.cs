using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Commands.Common;

using MediatR;
namespace KLMN.Application.Cities.Commands.DeleteCityDef;
public sealed class DeleteCityDefCommandHandler(IKLMNDbContext db) : IRequestHandler<DeleteCityDefCommand>
{
    public async Task Handle(DeleteCityDefCommand request, CancellationToken ct)
    {
        var item = await CityDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);

        // Şehir soft-delete edilse bile ilçeler var olan CityId değerlerini taşır.
        // Bu nedenle silinmemiş herhangi bir ilçe varsa işlemi reddet.
        await CityDefRules.EnsureNoTownsAsync(db, item.Id, deactivate: false, ct);
        await CityDefRules.EnsureNoMilitarySchoolsAsync(db, item.Id, deactivate: false, ct);
        await CityDefRules.EnsureNoMilitaryOfficesAsync(db, item.Id, deactivate: false, ct);
        await CityDefRules.EnsureNoHospitalsAsync(db, item.Id, false, ct);
        db.CityDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}
