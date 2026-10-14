using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Commands.Common;

using MediatR;
namespace KLMN.Application.Cities.Commands.DeleteCityDef;
public sealed class DeleteCityDefCommandHandler(IKLMNDbContext db) : IRequestHandler<DeleteCityDefCommand>
{
    public async Task Handle(DeleteCityDefCommand request, CancellationToken ct)
    {
        var item = await CityDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);

        db.CityDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}
