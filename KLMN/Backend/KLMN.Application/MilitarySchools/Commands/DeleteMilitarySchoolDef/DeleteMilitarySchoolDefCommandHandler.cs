using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Commands.Common;

using MediatR;

namespace KLMN.Application.MilitarySchools.Commands.DeleteMilitarySchoolDef;

/// <summary>Okulu EF Core soft-delete ve audit interceptor'ı üzerinden kaldırır.</summary>
public sealed class DeleteMilitarySchoolDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitarySchoolDefCommand>
{
    /// <summary>Kaydı xmin kontrolünden geçirip güvenli durum değişikliği yapar.</summary>
    public async Task Handle(DeleteMilitarySchoolDefCommand request, CancellationToken ct)
    {
        var school = await MilitarySchoolDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.MilitarySchoolDefs.Remove(school);
        await db.SaveChangesAsync(ct);

    }
}
