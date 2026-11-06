using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Provizyon durumlarının referans tablosu; kodlar sabittir.</summary>
public sealed class ProvisionStatusDefConfiguration : IEntityTypeConfiguration<ProvisionStatusDef>
{
    public void Configure(EntityTypeBuilder<ProvisionStatusDef> b)
    {
        b.ToTable("ProvisionStatusDefs");
        b.HasKey(x => x.Code);
        b.Property(x => x.Code).ValueGeneratedNever();
        b.Property(x => x.Name).HasMaxLength(60).IsRequired();
        b.Property(x => x.SortOrder).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.HasData(
            new ProvisionStatusDef { Code = ProvisionStatusCodes.New, Name = "Yeni", SortOrder = 10 },
            new ProvisionStatusDef { Code = ProvisionStatusCodes.Report, Name = "Rapor", SortOrder = 20 },
            new ProvisionStatusDef { Code = ProvisionStatusCodes.Cancelled, Name = "İptal", SortOrder = 30 },
            new ProvisionStatusDef { Code = ProvisionStatusCodes.Notification, Name = "Bildirim", SortOrder = 40 }
        );
    }
}
