using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Sevk kişisi kimlik numaraları, kısmi benzersiz indeksler ve kişisel veri sınırları.</summary>
public sealed class PersonConfiguration : BaseEntityConfiguration<Person>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Person> b)
    {
        b.ToTable("Persons");
        b.Property(x => x.TcKimlikNo).HasMaxLength(11);
        b.Property(x => x.YabanciKimlikNo).HasMaxLength(20);
        b.Property(x => x.Name).IsRequired().HasMaxLength(64);
        b.Property(x => x.Surname).IsRequired().HasMaxLength(64);
        b.Property(x => x.MotherName).HasMaxLength(64);
        b.Property(x => x.FatherName).HasMaxLength(64);
        b.Property(x => x.PhoneNumber).HasMaxLength(20);
        b.Property(x => x.Address).HasMaxLength(512);
        b.Property(x => x.BirthPlaceText).HasMaxLength(500);
        b.Property(x => x.BirthPlaceCode).HasMaxLength(250);
        b.Property(x => x.BirthDate).HasColumnType("date");
        b.HasIndex(x => x.TcKimlikNo).IsUnique().HasFilter("\"IsDeleted\" = false AND \"TcKimlikNo\" IS NOT NULL");
        b.HasIndex(x => x.YabanciKimlikNo).IsUnique().HasFilter("\"IsDeleted\" = false AND \"YabanciKimlikNo\" IS NOT NULL");
        b.HasOne<KLMN.Domain.Entities.Geography.CountryDef>().WithMany().HasForeignKey(x => x.NationalityId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<KLMN.Domain.Entities.Geography.TownDef>().WithMany().HasForeignKey(x => x.TownOfBirthId).OnDelete(DeleteBehavior.Restrict);
    }
}
