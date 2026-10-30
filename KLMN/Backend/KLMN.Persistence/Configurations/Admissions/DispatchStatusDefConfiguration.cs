using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>BaseEntity audit/soft-delete/xmin ile duruma özel sabit başlangıç kayıtları.</summary>
public sealed class DispatchStatusDefConfiguration : BaseEntityConfiguration<DispatchStatusDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DispatchStatusDef> builder)
    {
        builder.ToTable("DispatchStatusDefs");
        builder.Property(x=>x.Code).IsRequired();
        builder.HasIndex(x=>x.Code).IsUnique();
        builder.Property(x=>x.Name).IsRequired().HasMaxLength(80);
        builder.HasIndex(x=>x.SortOrder);

        // HasData migration ile eklenir; elle ekran olmadan DB üzerinden düzenlenebilir.
        var created=new DateTime(2026,10,30,0,0,0,DateTimeKind.Utc);
        builder.HasData(
            new DispatchStatusDef {Id=Guid.Parse("a90431ff-a1b0-4ed0-87f9-71aa99004101"),CreatedDate=created,Code=1,Name="Yeni",SortOrder=10},
            new DispatchStatusDef {Id=Guid.Parse("a90431ff-a1b0-4ed0-87f9-71aa99004102"),CreatedDate=created,Code=2,Name="Provizyon",SortOrder=20},
            new DispatchStatusDef {Id=Guid.Parse("a90431ff-a1b0-4ed0-87f9-71aa99004103"),CreatedDate=created,Code=3,Name="Rapor",SortOrder=30},
            new DispatchStatusDef {Id=Guid.Parse("a90431ff-a1b0-4ed0-87f9-71aa99004104"),CreatedDate=created,Code=4,Name="İptal",SortOrder=40},
            new DispatchStatusDef {Id=Guid.Parse("a90431ff-a1b0-4ed0-87f9-71aa99004105"),CreatedDate=created,Code=5,Name="Bildirim",SortOrder=50}
        );
    }
}
