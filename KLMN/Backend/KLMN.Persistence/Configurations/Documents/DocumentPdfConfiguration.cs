using KLMN.Domain.Entities.Documents;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Documents;

/// <summary>DocumentPdfs: içerik GUID'si ve PDF metaverisi; binary dosya özel disktedir.</summary>
public sealed class DocumentPdfConfiguration : BaseEntityConfiguration<DocumentPdf>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DocumentPdf> builder)
    {
        builder.ToTable("DocumentPdfs");
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.PdfDataObjectId).IsRequired();
        builder.Property(x => x.SizeBytes).IsRequired();
        builder.HasIndex(x => x.PdfDataObjectId).IsUnique();
        builder.HasIndex(x => x.FileName);
    }
}

