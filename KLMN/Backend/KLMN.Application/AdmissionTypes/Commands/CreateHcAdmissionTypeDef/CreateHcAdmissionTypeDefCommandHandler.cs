using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AdmissionTypes.Commands.Common;
using KLMN.Application.AdmissionTypes.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.CreateHcAdmissionTypeDef;

/// <summary>Başvuru grubu kaydı ekler; audit ve xmin ortak altyapıdadır.</summary>
public sealed class CreateHcAdmissionTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateHcAdmissionTypeDefCommand, HcAdmissionTypeDefMutationResponse>
{
    /// <summary>Tekil kodu doğrular ve tüm boolean seçimleri eksiksiz kaydeder.</summary>
    public async Task<HcAdmissionTypeDefMutationResponse> Handle(
        CreateHcAdmissionTypeDefCommand request, CancellationToken ct)
    {
        await HcAdmissionTypeDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var item = new HcAdmissionTypeDef
        {
            Code = request.Code,
            Name = request.Name.Trim(),
            ForcesCommandRequired = request.ForcesCommandRequired,
            RankRequired = request.RankRequired,
            MilitaryClassRequired = request.MilitaryClassRequired,
            MilitaryUnitRequired = request.MilitaryUnitRequired,
            SenderChairRequired = request.SenderChairRequired,
            MilitaryOfficeRequired = request.MilitaryOfficeRequired,
            RegistryNumberRequired = request.RegistryNumberRequired,
            MilitarySchoolRequired = request.MilitarySchoolRequired,
            LastNominationDateRequired = request.LastNominationDateRequired,
            HeightWeightRequired = request.HeightWeightRequired,
            NthAdmissionRequired = request.NthAdmissionRequired,
            DispatchDocumentDateRequired = request.DispatchDocumentDateRequired,
            DispatchDocumentNoRequired = request.DispatchDocumentNoRequired,
            PertemApprovalRequired = request.PertemApprovalRequired,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.HcAdmissionTypeDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new HcAdmissionTypeDefMutationResponse(item.Id, item.Version);
    }
}
