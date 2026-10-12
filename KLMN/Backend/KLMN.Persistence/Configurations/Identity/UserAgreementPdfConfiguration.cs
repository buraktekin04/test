using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// PDF dosyasının kendisini değil, kullanıcı ilişkisi ve disk yolunu tutan
/// UserAgreementPdfs tablosunun EF Core eşlemesini tanımlar.
/// </summary>
public sealed class UserAgreementPdfConfiguration : BaseEntityConfiguration<UserAgreementPdf>
{
    /// <summary>Kolon uzunlukları, kullanıcı FK'sı ve listeleme indekslerini yapılandırır.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<UserAgreementPdf> builder)
    {
        builder.ToTable("UserAgreementPdfs");

        // Disk anahtarı uygulamanın kendi ürettiği güvenli GUID dosya adıdır.
        builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.RelativePath).IsRequired().HasMaxLength(512);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SizeBytes).IsRequired();

        // Kullanıcı fiziksel silinirse PDF metadata kayıtlarını otomatik silmeyiz.
        builder.HasOne(x => x.User)
            .WithMany(x => x.AgreementPdfs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.CreatedDate });
        builder.HasIndex(x => x.RelativePath).IsUnique();
    }
}
