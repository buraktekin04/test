using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ReasonForExaminations.Responses;
/// <summary>Ana listede her muayene sebebinin ilgili aktif tanımlarını da sunar.</summary>
public sealed record ReasonForExaminationDefResponse(
    Guid Id,string Code,string Description,bool DispatchRequired,bool ClassificationExamination,bool Objectionable,
    DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate,
    IReadOnlyList<string> ReportTypes,IReadOnlyList<string> UnusedAdmissionTypes,
    IReadOnlyList<string> Procedures,IReadOnlyList<string> Branches)
{
    /// <summary>İlişki alanları aynı kayıtta bulunduğu için kullanıcı ekrana girmeden var/yok görebilir.</summary>
    public static ReasonForExaminationDefResponse From(ReasonForExaminationDef x) => new(
        x.Id,x.Code,x.Description,x.DispatchRequired,x.ClassificationExamination,x.Objectionable,
        x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate,
        x.ReportTypes.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.ReportType.Name).OrderBy(x=>x).ToArray(),
        x.UnusedAdmissionTypes.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.AdmissionType.Name).OrderBy(x=>x).ToArray(),
        x.Procedures.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.Procedure.Name).OrderBy(x=>x).ToArray(),
        x.Branches.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.Branch.Name).OrderBy(x=>x).ToArray());
}
