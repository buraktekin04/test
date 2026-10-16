using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.UpdateHcAdmissionTypeDef;

/// <summary>Başvuru grubunun güncellenmesi için tüm zorunluluk ayarlarını içerir.</summary>
public sealed record UpdateHcAdmissionTypeDefCommand(
    Guid Id,
    uint Version,
    int Code,
    string Name,
    bool ForcesCommandRequired,
    bool RankRequired,
    bool MilitaryClassRequired,
    bool MilitaryUnitRequired,
    bool SenderChairRequired,
    bool MilitaryOfficeRequired,
    bool RegistryNumberRequired,
    bool MilitarySchoolRequired,
    bool LastNominationDateRequired,
    bool HeightWeightRequired,
    bool NthAdmissionRequired,
    bool DispatchDocumentDateRequired,
    bool DispatchDocumentNoRequired,
    bool PertemApprovalRequired,
    DateTime? ValidFrom,
    DateTime? ValidTo)
    : IRequest<HcAdmissionTypeDefMutationResponse>;
