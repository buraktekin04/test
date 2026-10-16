using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Başvuru grubu alanlarını ortak effective-dated ve xmin altyapısına bağlar.</summary>
public sealed class HcAdmissionTypeDefConfiguration : EffectiveDatedEntityConfiguration<HcAdmissionTypeDef>
{
    /// <summary>Code int tipinde tekil; zorunluluk ayarları ayrı bool kolonlardır.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<HcAdmissionTypeDef> builder)
    {
        builder.ToTable("HcAdmissionTypeDefs");
        builder.Property(x => x.Code).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.Property(x => x.ForcesCommandRequired).IsRequired();
        builder.Property(x => x.RankRequired).IsRequired();
        builder.Property(x => x.MilitaryClassRequired).IsRequired();
        builder.Property(x => x.MilitaryUnitRequired).IsRequired();
        builder.Property(x => x.SenderChairRequired).IsRequired();
        builder.Property(x => x.MilitaryOfficeRequired).IsRequired();
        builder.Property(x => x.RegistryNumberRequired).IsRequired();
        builder.Property(x => x.MilitarySchoolRequired).IsRequired();
        builder.Property(x => x.LastNominationDateRequired).IsRequired();
        builder.Property(x => x.HeightWeightRequired).IsRequired();
        builder.Property(x => x.NthAdmissionRequired).IsRequired();
        builder.Property(x => x.DispatchDocumentDateRequired).IsRequired();
        builder.Property(x => x.DispatchDocumentNoRequired).IsRequired();
        builder.Property(x => x.PertemApprovalRequired).IsRequired();
    }
}
