using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Countries.Commands.Common;

using MediatR;
namespace KLMN.Application.Countries.Commands.DeleteCountryDef;
public sealed class DeleteCountryDefCommandHandler(IKLMNDbContext db) : IRequestHandler<DeleteCountryDefCommand>
{
    public async Task Handle(DeleteCountryDefCommand request, CancellationToken ct)
    {
        var item = await CountryDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await CountryDefRules.EnsureNoCitiesAsync(db, item.Id, false, ct);
        db.CountryDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}
