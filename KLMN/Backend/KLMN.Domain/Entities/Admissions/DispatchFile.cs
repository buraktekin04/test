using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sevk kaydına ait boyutu sınırlandırılmış, erişimi yetkilendirilmiş PDF/görsel eki.</summary>
public sealed class DispatchFile : BaseEntity
{
    public Guid DispatchId { get; set; }
    public Dispatch Dispatch { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] DataContent { get; set; } = Array.Empty<byte>();
    public int FileSize { get; set; }
}
