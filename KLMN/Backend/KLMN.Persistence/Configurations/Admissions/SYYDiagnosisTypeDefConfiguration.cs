using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Eski integer kodun tekilliği ve tanı referanslarının kararlılığı.</summary>
public sealed class SYYDiagnosisTypeDefConfiguration:BaseEntityConfiguration<SYYDiagnosisTypeDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisTypeDef> b)
    {
        b.ToTable("SYYDiagnosisTypeDefs");
        b.Property(x=>x.Name).HasMaxLength(150).IsRequired();
        b.Property(x=>x.Code).ValueGeneratedNever();
        b.HasIndex(x=>x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
