using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Cinsiyet sözlüğü; eski enum kodları 1/2/3 korunur.</summary>
public sealed class GenderDefConfiguration : BaseEntityConfiguration<GenderDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GenderDef> builder)
    {
        builder.ToTable("GenderDefs");
        builder.Property(x=>x.Code).IsRequired();
        builder.HasIndex(x=>x.Code).IsUnique();
        builder.Property(x=>x.Name).HasMaxLength(60).IsRequired();
        builder.HasIndex(x=>x.SortOrder);

        var created=new DateTime(2026,10,30,0,0,0,DateTimeKind.Utc);
        builder.HasData(
            new GenderDef {Id=Guid.Parse("ce1967d5-5350-494f-a1da-764da8000001"),CreatedDate=created,Code=1,Name="Erkek",SortOrder=10},
            new GenderDef {Id=Guid.Parse("ce1967d5-5350-494f-a1da-764da8000002"),CreatedDate=created,Code=2,Name="Kadın",SortOrder=20},
            new GenderDef {Id=Guid.Parse("ce1967d5-5350-494f-a1da-764da8000003"),CreatedDate=created,Code=3,Name="Belirtilmemiş",SortOrder=30}
        );
    }
}
