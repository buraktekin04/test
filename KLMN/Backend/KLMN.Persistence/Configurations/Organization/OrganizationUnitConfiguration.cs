using KLMN.Domain.Entities.Organization;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Organization;

/// <summary>
/// OrganizationUnit entity EF Core configuration'ıdır.
/// </summary>
internal sealed class OrganizationUnitConfiguration
    : BaseEntityConfiguration<OrganizationUnit>
{
    public override void Configure(
        EntityTypeBuilder<OrganizationUnit> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrganizationUnits");

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");

        builder.HasOne(x => x.ParentOrganizationUnit)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentOrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
