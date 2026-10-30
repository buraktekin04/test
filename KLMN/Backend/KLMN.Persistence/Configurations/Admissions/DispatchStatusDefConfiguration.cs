using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Sevk durumları veritabanı sözlüğü; migration HasData ile başlangıç durumları oluşturulur.</summary>
public sealed class DispatchStatusDefConfiguration : IEntityTypeConfiguration<DispatchStatusDef>
{
    public void Configure(EntityTypeBuilder<DispatchStatusDef> builder)
    {
        builder.ToTable("DispatchStatusDefs");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).ValueGeneratedNever();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(80);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.HasIndex(x => x.SortOrder);
        builder.HasData(
            new DispatchStatusDef { Code=1, Name="Yeni", SortOrder=10, IsActive=true },
            new DispatchStatusDef { Code=2, Name="Provizyon", SortOrder=20, IsActive=true },
            new DispatchStatusDef { Code=3, Name="Rapor", SortOrder=30, IsActive=true },
            new DispatchStatusDef { Code=4, Name="İptal", SortOrder=40, IsActive=true },
            new DispatchStatusDef { Code=5, Name="Bildirim", SortOrder=50, IsActive=true }
        );
    }
}
