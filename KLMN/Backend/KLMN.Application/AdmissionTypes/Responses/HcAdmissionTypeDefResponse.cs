using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.AdmissionTypes.Responses;

/// <summary>Başvuru grubu liste/detay DTO'su; 14 zorunluluk alanı eşlenir.</summary>
public sealed class HcAdmissionTypeDefResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public string Name { get; init; } = string.Empty;
    /// <summary>Başvuru grubu formundaki "Kuvvet Gerekir" seçimi; API alan adı korunur.</summary>
    public bool ForcesCommandRequired { get; init; }
    public bool RankRequired { get; init; }
    public bool MilitaryClassRequired { get; init; }
    public bool MilitaryUnitRequired { get; init; }
    public bool SenderChairRequired { get; init; }
    public bool MilitaryOfficeRequired { get; init; }
    public bool RegistryNumberRequired { get; init; }
    public bool MilitarySchoolRequired { get; init; }
    /// <summary>Başvuru grubu formundaki "Nasbı Gerekir" seçimi; API alan adı korunur.</summary>
    public bool LastNominationDateRequired { get; init; }
    public bool HeightWeightRequired { get; init; }
    public bool NthAdmissionRequired { get; init; }
    /// <summary>Başvuru grubu formundaki "Sevk Evrak Tarihi Gerekir" seçimi; API alan adı korunur.</summary>
    public bool DispatchDocumentDateRequired { get; init; }
    /// <summary>Başvuru grubu formundaki "Sevk Evrak Sayısı Gerekir" seçimi; API alan adı korunur.</summary>
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

}
