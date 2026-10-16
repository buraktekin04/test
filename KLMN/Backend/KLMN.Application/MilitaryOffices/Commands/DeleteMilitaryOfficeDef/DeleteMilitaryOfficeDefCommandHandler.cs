using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryOffices.Commands.Common;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.DeleteMilitaryOfficeDef;

/// <summary>Şube Delete işlemini ilişki ve concurrency kurallarıyla yürütür.</summary>
public sealed class DeleteMilitaryOfficeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitaryOfficeDefCommand>
{
    /// <summary>İstek doğrulandıktan sonra EF Core üzerinden işlem yapar.</summary>
    public async Task Handle(DeleteMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryOfficeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.MilitaryOfficeDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
