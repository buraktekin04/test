using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Dosya eki meta verisi; silinen sevkler geçmiş kaydı kaybetmez.</summary>
public sealed class DispatchFileConfiguration : BaseEntityConfiguration<DispatchFile>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DispatchFile> b)
    {
        b.ToTable("DispatchFiles");
        b.Property(x => x.FileName).IsRequired().HasMaxLength(180);
        b.Property(x => x.ContentType).IsRequired().HasMaxLength(80);
        b.Property(x => x.DataContent).IsRequired();
        b.HasOne(x => x.Dispatch).WithMany(x => x.Files).HasForeignKey(x => x.DispatchId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.DispatchId);
    }
}
