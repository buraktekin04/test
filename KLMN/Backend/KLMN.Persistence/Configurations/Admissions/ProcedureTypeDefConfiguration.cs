using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Eski enum kodlarını bozmayan dinamik tetkik türü tablo tanımı.</summary>
public sealed class ProcedureTypeDefConfiguration : BaseEntityConfiguration<ProcedureTypeDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProcedureTypeDef> b)
    {
        b.ToTable("ProcedureTypeDefs");
        b.Property(x=>x.Code).ValueGeneratedNever();
        b.Property(x=>x.Name).IsRequired().HasMaxLength(200);
        // FK artık GUID kimliğine bağlı; silinmiş kodu tekrar kullanmak mümkündür.
        b.HasIndex(x=>x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
