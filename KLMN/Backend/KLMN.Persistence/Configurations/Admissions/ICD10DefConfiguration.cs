using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>ICD10 Tanımları: kod benzersizliği, soft delete ve BaseEntity mapping.</summary>
public sealed class ICD10DefConfiguration:BaseEntityConfiguration<ICD10Def>
{
 protected override void ConfigureEntity(EntityTypeBuilder<ICD10Def> b)
 {
  b.ToTable("ICD10Defs");
  b.Property(x=>x.Code).IsRequired().HasMaxLength(32);
  b.Property(x=>x.Name).IsRequired().HasMaxLength(500);
  b.HasIndex(x=>x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
