using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>
/// Başvuru grubu tanımı. Fotoğraftaki HcAdmissionTypeDef modelinin Code(int),
/// Name ve 14 adet zorunluluk ayarını taşır. Kimlik, audit, aktiflik, soft-delete
/// ve xmin BaseEntity; geçerlilik tarihleri EffectiveDatedEntity tarafından sağlanır.
/// </summary>
public sealed class HcAdmissionTypeDef : EffectiveDatedEntity
{
    /// <summary>Başvuru grubunun silinmemiş kayıtlar arasında tekil sayısal kodudur.</summary>
    public int Code { get; set; }

    /// <summary>Başvuru grubunun adıdır.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Bu hasta grubunun kullanılmadığı muayene sebepleri.</summary>
    public ICollection<ReasonForExamUnusedHcAdmissionType> UnusedExaminationReasons { get; set; } = new List<ReasonForExamUnusedHcAdmissionType>();


    /// <summary>Başvuruda Kuvvet Komutanlığı alanının zorunlu olup olmayacağını belirler.</summary>
    public bool ForcesCommandRequired { get; set; }

    /// <summary>Başvuruda Rütbe alanının zorunlu olup olmayacağını belirler.</summary>
    public bool RankRequired { get; set; }

    /// <summary>Başvuruda Askerî Sınıf alanının zorunlu olup olmayacağını belirler.</summary>
    public bool MilitaryClassRequired { get; set; }

    /// <summary>Başvuruda Birlik alanının zorunlu olup olmayacağını belirler.</summary>
    public bool MilitaryUnitRequired { get; set; }

    /// <summary>Başvuruda Gönderen Makam alanının zorunlu olup olmayacağını belirler.</summary>
    public bool SenderChairRequired { get; set; }

    /// <summary>Başvuruda Askerlik Şubesi alanının zorunlu olup olmayacağını belirler.</summary>
    public bool MilitaryOfficeRequired { get; set; }

    /// <summary>Başvuruda Sicil Numarası alanının zorunlu olup olmayacağını belirler.</summary>
    public bool RegistryNumberRequired { get; set; }

    /// <summary>Başvuruda Askerî Okul alanının zorunlu olup olmayacağını belirler.</summary>
    public bool MilitarySchoolRequired { get; set; }

    /// <summary>Başvuruda Son Adaylık Tarihi alanının zorunlu olup olmayacağını belirler.</summary>
    public bool LastNominationDateRequired { get; set; }

    /// <summary>Başvuruda Boy ve Kilo alanının zorunlu olup olmayacağını belirler.</summary>
    public bool HeightWeightRequired { get; set; }

    /// <summary>Başvuruda Kaçıncı Başvuru alanının zorunlu olup olmayacağını belirler.</summary>
    public bool NthAdmissionRequired { get; set; }

    /// <summary>Başvuruda Sevk Belgesi Tarihi alanının zorunlu olup olmayacağını belirler.</summary>
    public bool DispatchDocumentDateRequired { get; set; }

    /// <summary>Başvuruda Sevk Belgesi Numarası alanının zorunlu olup olmayacağını belirler.</summary>
    public bool DispatchDocumentNoRequired { get; set; }

    /// <summary>Başvuruda PERTEM Onayı alanının zorunlu olup olmayacağını belirler.</summary>
    public bool PertemApprovalRequired { get; set; }
    /// <summary>Bu başvuru grubuna bağlı ICD10 SYY tanı istatistikleri.</summary>
    public ICollection<ICD10Diagnostic> ICD10Diagnostics {get;set;}=new List<ICD10Diagnostic>();
}
