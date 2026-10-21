using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Tetkik Tanımları; tarihleri ortak EffectiveDatedEntity mapping'iyle ValidFrom/ValidTo kolonlarına bağlar.</summary>
public sealed class ProcedureDefConfiguration : EffectiveDatedEntityConfiguration<ProcedureDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProcedureDef> b)
    {
        b.ToTable("ProcedureDefs");
        b.Property(x=>x.ProcedureCode).IsRequired().HasMaxLength(100);
        b.Property(x=>x.Name).IsRequired().HasMaxLength(250);
        b.Property(x=>x.RequiresNumericResult).IsRequired();
        b.Property(x=>x.RequiresReferenceRange).IsRequired();
        b.Property(x=>x.RequiresDeviationInfo).IsRequired();
        b.Property(x=>x.ProcedureType).IsRequired();
        b.HasIndex(x=>x.ProcedureCode).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        b.HasOne(x=>x.TypeDefinition).WithMany(x=>x.Procedures)
          .HasForeignKey(x=>x.ProcedureType).HasPrincipalKey(x=>x.Code)
          .OnDelete(DeleteBehavior.Restrict);
    }
}
