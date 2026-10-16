using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AdmissionTypes.Commands.Common;
using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.UpdateHcAdmissionTypeDef;

/// <summary>Başvuru grubu kaydı günceller; audit ve xmin ortak altyapıdadır.</summary>
public sealed class UpdateHcAdmissionTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateHcAdmissionTypeDefCommand, HcAdmissionTypeDefMutationResponse>
{
    /// <summary>Tekil kodu doğrular ve tüm boolean seçimleri eksiksiz kaydeder.</summary>
    public async Task<HcAdmissionTypeDefMutationResponse> Handle(
        UpdateHcAdmissionTypeDefCommand request, CancellationToken ct)
    {
        var item = await HcAdmissionTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await HcAdmissionTypeDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code;
        item.Name = request.Name.Trim();
        item.ForcesCommandRequired = request.ForcesCommandRequired;
        item.RankRequired = request.RankRequired;
        item.MilitaryClassRequired = request.MilitaryClassRequired;
        item.MilitaryUnitRequired = request.MilitaryUnitRequired;
        item.SenderChairRequired = request.SenderChairRequired;
        item.MilitaryOfficeRequired = request.MilitaryOfficeRequired;
        item.RegistryNumberRequired = request.RegistryNumberRequired;
        item.MilitarySchoolRequired = request.MilitarySchoolRequired;
        item.LastNominationDateRequired = request.LastNominationDateRequired;
        item.HeightWeightRequired = request.HeightWeightRequired;
        item.NthAdmissionRequired = request.NthAdmissionRequired;
        item.DispatchDocumentDateRequired = request.DispatchDocumentDateRequired;
        item.DispatchDocumentNoRequired = request.DispatchDocumentNoRequired;
        item.PertemApprovalRequired = request.PertemApprovalRequired;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new HcAdmissionTypeDefMutationResponse(item.Id, item.Version);
    }
}
