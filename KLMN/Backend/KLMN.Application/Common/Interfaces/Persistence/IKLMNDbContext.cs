using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.Auditing;
using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Domain.Entities.Admissions;
using KLMN.Domain.Entities.PurchaseNumbers;
using KLMN.Domain.Entities.Geography;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Common.Interfaces.Persistence;

/// <summary>Application katmanının KLMN veritabanına erişim sözleşmesidir.</summary>
public interface IKLMNDbContext
{
    DbSet<User> Users { get; }

    /// <summary>Audit geçmişi sorgulama modeli; interceptor haricinde yazılmaz.</summary>
    DbSet<AuditLog> AuditLogs { get; }

    /// <summary>
    /// Kullanıcıya bağlı PDF meta verisi kayıtlarını sağlar.
    /// PDF içerikleri veritabanında değil, yapılandırılmış özel dosya dizininde tutulur.
    /// </summary>
    DbSet<UserAgreementPdf> UserAgreementPdfs { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserPermission> UserPermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<ForcesCommandDef> ForcesCommandDefs { get; }

    /// <summary>Başvuru gruplarının DbSet sorgu/değiştirme kümesi.</summary>
    DbSet<HcAdmissionTypeDef> HcAdmissionTypeDefs { get; }

    /// <summary>Askerlik şubelerinin CQRS işlem veri kümesi.</summary>
    DbSet<MilitaryOfficeDef> MilitaryOfficeDefs { get; }

    /// <summary>Askerî okulların sorgu ve komut işlemleri için veri kümesi.</summary>
    DbSet<MilitarySchoolDef> MilitarySchoolDefs { get; }

    /// <summary>Askerî sınıf entity'lerinin sorgulandığı veri kümesi.</summary>
    DbSet<MilitaryClassDef> MilitaryClassDefs { get; }

    /// <summary>Askerî sınıf grupları.</summary>
    DbSet<MilitaryClassGroupDef> MilitaryClassGroupDefs { get; }

    /// <summary>Kuvvet, askerî sınıf ve grup arasındaki üçlü eşleştirmeler.</summary>
    DbSet<MilitaryClassGroupRelation> MilitaryClassGroupRelations { get; }

    DbSet<PurchaseNumberDef> PurchaseNumberDefs { get; }
    DbSet<CountryDef> CountryDefs { get; }
    DbSet<CityDef> CityDefs { get; }

    /// <summary>İlçe tanımlarını sorgulama/değiştirme veri kümesi.</summary>
    DbSet<TownDef> TownDefs { get; }

    DbSet<MilitaryUnitDef> MilitaryUnitDefs { get; }

    /// <summary>
    /// save changes async işlemini çağıran katmana belirtilen sözleşmeyle sunar.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
