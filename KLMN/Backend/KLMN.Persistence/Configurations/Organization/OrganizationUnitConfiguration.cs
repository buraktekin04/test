using KLMN.Domain.Entities.Organization;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Organization;

/// <summary>OrganizationUnit hiyerarşi ve index mapping'ini tanımlar.</summary>
public sealed class OrganizationUnitConfiguration : BaseEntityConfiguration<OrganizationUnit>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OrganizationUnit> builder)
    {
        builder.ToTable("OrganizationUnits");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.ParentOrganizationUnit)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentOrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        builder.HasIndex(x => x.ParentOrganizationUnitId);
        builder.HasIndex(x => x.Name);
    }
}
