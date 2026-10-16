using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.AdmissionTypes.Responses;

/// <summary>Başvuru grubu liste/detay DTO'su; 14 zorunluluk alanı eşlenir.</summary>
public sealed class HcAdmissionTypeDefResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool ForcesCommandRequired { get; init; }
    public bool RankRequired { get; init; }
    public bool MilitaryClassRequired { get; init; }
    public bool MilitaryUnitRequired { get; init; }
    public bool SenderChairRequired { get; init; }
    public bool MilitaryOfficeRequired { get; init; }
    public bool RegistryNumberRequired { get; init; }
    public bool MilitarySchoolRequired { get; init; }
    public bool LastNominationDateRequired { get; init; }
    public bool HeightWeightRequired { get; init; }
    public bool NthAdmissionRequired { get; init; }
    public bool DispatchDocumentDateRequired { get; init; }
    public bool DispatchDocumentNoRequired { get; init; }
    public bool PertemApprovalRequired { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>EF Core tarafından SQL'e çevrilen ortak liste/detay projeksiyonudur.</summary>
    public static Expression<Func<HcAdmissionTypeDef, HcAdmissionTypeDefResponse>> Projection => x =>
        new HcAdmissionTypeDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            ForcesCommandRequired = x.ForcesCommandRequired,
            RankRequired = x.RankRequired,
            MilitaryClassRequired = x.MilitaryClassRequired,
            MilitaryUnitRequired = x.MilitaryUnitRequired,
            SenderChairRequired = x.SenderChairRequired,
            MilitaryOfficeRequired = x.MilitaryOfficeRequired,
            RegistryNumberRequired = x.RegistryNumberRequired,
            MilitarySchoolRequired = x.MilitarySchoolRequired,
            LastNominationDateRequired = x.LastNominationDateRequired,
            HeightWeightRequired = x.HeightWeightRequired,
            NthAdmissionRequired = x.NthAdmissionRequired,
            DispatchDocumentDateRequired = x.DispatchDocumentDateRequired,
            DispatchDocumentNoRequired = x.DispatchDocumentNoRequired,
            PertemApprovalRequired = x.PertemApprovalRequired,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            IsActive = x.IsActive, CreatedDate = x.CreatedDate,
            CreatedBy = x.CreatedBy, UpdatedDate = x.UpdatedDate,
            UpdatedBy = x.UpdatedBy, Version = x.Version
        };
}
